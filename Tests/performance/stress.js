import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '1m', target: 10 },
        { duration: '2m', target: 50 },  
        { duration: '1m', target: 0 }
    ],
    thresholds: {
        http_req_failed: ['rate<0.1'], // <10%
        http_req_duration: ['p(95)<1000'] // 95% requests sub 1s
    }
};

export default function () {
    let res = http.get('http://localhost:5145/api/health');
    check(res, { 'health 200': (r) => r.status === 200 });
    sleep(1);
}
