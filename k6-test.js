import http from 'k6/http';
import { check } from 'k6';

export default function () {
    let res = http.get('https://185.196.21.234:8000/api/movies');
    check(res, {
        'status is 200': (r) => r.status === 200
    });
}