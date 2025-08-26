import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '30s', target: 5 },   // ramp-up la 5 VUs
        { duration: '1m', target: 10 },   // ramp-up la 10 VUs
        { duration: '1m', target: 20 },   // ramp-up la 20 VUs
        { duration: '30s', target: 0 }    // ramp-down
    ],
    thresholds: {
        http_req_failed: ['rate<0.05'], // <5% 
        http_req_duration: ['p(95)<600'] // 95% requests sub 600ms
    }
};

export default function () {
    let resHealth = http.get('http://localhost:5145/api/health');
    check(resHealth, {
        'health 200': (r) => r.status === 200
    });

    let resList = http.get('http://localhost:5145/api/users'); 
    check(resList, {
        'list 200': (r) => r.status === 200
    });

    sleep(1); 
}
