import http from 'k6/http';
import { sleep, check } from 'k6';

export const options = {
  vus: 1,
  duration: '20s',
  thresholds: {
    http_req_failed: ['rate<0.01'],        // <1% erori
    http_req_duration: ['p(95)<500'],      // 95% < 500ms
  },
};

const API = __ENV.API_BASE || 'http://localhost:5145';

export default function () {
  const res = http.get(`${API}/api/health`);
  check(res, {
    'health is 200': (r) => r.status === 200,
  });
  sleep(1);
}
