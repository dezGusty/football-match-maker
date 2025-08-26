// ...existing code...
import http from 'k6/http';
import { check, group, sleep } from 'k6';
import { Trend, Rate } from 'k6/metrics';
import { SharedArray } from 'k6/data';

export let options = {
  scenarios: {
    baseline: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '30s', target: 30 },
        { duration: '2m', target: 30 },
        { duration: '30s', target: 0 },
      ],
      exec: 'baselineScenario',
    },
    spike: {
      executor: 'ramping-vus',
      startVUs: 0,
      stages: [
        { duration: '10s', target: 0 },
        { duration: '20s', target: 150 },
        { duration: '40s', target: 150 },
        { duration: '20s', target: 0 },
      ],
      exec: 'spikeScenario',
    },
    soak: {
      executor: 'constant-vus',
      vus: 20,
      duration: '10m',
      exec: 'soakScenario',
    },
  },
  thresholds: {
    http_req_duration: ['p(95)<700', 'p(99)<1500'],
    'http_req_failed': ['rate<0.02'],
    //'matches_creation_rate{type:create}': ['rate>0.9'],
  },
  noConnectionReuse: false,
};

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5145';
const USERNAME = __ENV.TEST_USER || 'admin@gmail.com';
const PASSWORD = __ENV.TEST_PASS || 'default123';

const reqTrend = new Trend('req_duration');
const errorRate = new Rate('http_req_failed');
//const createRate = new Rate('matches_creation_rate{type:create}');

// seed data for payloads (kept in memory across VUs)
const teams = new SharedArray('teams', function () {
  return [
    'Red Lions', 'Blue Tigers', 'Green Hawks', 'Yellow Eagles',
    'Silver Foxes', 'Black Panthers', 'White Wolves', 'Orange Bulls',
  ];
});

function randomInt(min, max) {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}
function randomChoice(arr) {
  return arr[randomInt(0, arr.length - 1)];
}

export function setup() {
  // Optional: perform authentication once and return token for VUs
  if (USERNAME && PASSWORD) {
    const payload = JSON.stringify({ username: USERNAME, password: PASSWORD });
    const res = http.post(`${BASE_URL}/api/auth/login`, payload, {
      headers: { 'Content-Type': 'application/json' },
    });
    if (res.status === 200 && res.json('token')) {
      return { token: res.json('token') };
    }
    return { token: null };
  }
  return { token: null };
}

function authHeaders(token) {
  const h = { 'Content-Type': 'application/json' };
  if (token) h['Authorization'] = `Bearer ${token}`;
  return h;
}

export function baselineScenario(data) {
  mainFlow(data);
}
export function spikeScenario(data) {
  // spike users do more reads and occasional writes
  mainFlow(data, { createProbability: 0.05, joinProbability: 0.15 });
}
export function soakScenario(data) {
  // long-running steady traffic
  mainFlow(data, { createProbability: 0.02, joinProbability: 0.08 });
}

function mainFlow(data, opts = {}) {
  opts.createProbability = opts.createProbability || 0.03;
  opts.joinProbability = opts.joinProbability || 0.1;

  group('Health check', function () {
    const res = http.get(`${BASE_URL}/api/health`);
    reqTrend.add(res.timings.duration);
    const ok = check(res, { 'health 200': (r) => r.status === 200 });
    if (!ok) errorRate.add(1);
  });

  group('List matches & view match', function () {
    const res = http.get(`${BASE_URL}/api/matches`);
    reqTrend.add(res.timings.duration);
    const okList = check(res, {
      'list matches 200': (r) => r.status === 200,
      'list not empty or 200': (r) => r.status === 200,
    });
    if (!okList) errorRate.add(1);

    if (res.status === 200) {
      const list = res.json();
      if (Array.isArray(list) && list.length > 0) {
        const m = randomChoice(list);
        const r2 = http.get(`${BASE_URL}/api/matches/${m.id}`);
        reqTrend.add(r2.timings.duration);
        const okGet = check(r2, { 'get match 200': (r) => r.status === 200 });
        if (!okGet) errorRate.add(1);
      }
    }
  });

  // occasionally create a match
  // if (Math.random() < opts.createProbability) {
  //   group('Create match', function () {
  //     const payload = JSON.stringify({
  //       homeTeam: randomChoice(teams),
  //       awayTeam: randomChoice(teams),
  //       date: new Date(Date.now() + randomInt(1, 7) * 24 * 3600 * 1000).toISOString(),
  //       maxPlayers: randomInt(10, 22),
  //     });
  //     const res = http.post(`${BASE_URL}/api/matches`, payload, {
  //       headers: authHeaders(data.token),
  //     });
  //     reqTrend.add(res.timings.duration);
  //     const ok = check(res, { 'create 201 or 200': (r) => r.status === 201 || r.status === 200 });
  //     createRate.add(ok ? 1 : 0);
  //     if (!ok) errorRate.add(1);
  //     if (ok && res.json('id')) {
  //       // try to join the created match
  //       const id = res.json('id');
  //       const r2 = http.post(`${BASE_URL}/api/matches/${id}/join`, null, {
  //         headers: authHeaders(data.token),
  //       });
  //       reqTrend.add(r2.timings.duration);
  //       check(r2, { 'join created match': (r) => r.status === 200 || r.status === 201 });
  //     }
  //   });
  // }



  // random user think time
  sleep(1 + Math.random() * 3);
}
// ...existing code...