import http from 'k6/http';
import { check } from 'k6';
import { sleep } from 'k6';

export let options = {
  stages: [
    { duration: '1m', target: 200 },
  ],
};

export default function () {
  const url = 'http://localhost:8081/baskets';
  const payload = JSON.stringify({
    userId: "c4836ece-eae0-4adb-a009-7e2cee943ad1",
    products: [
      {
        productId: "cfff968a-0d0c-40d0-9b43-13acccd54415",
        productName: "Tuna",
        productCategory: randomCategory(1),
        quantity: randomQuantity(1)
      },
      {
        productId: "c0c30c6f-ed10-4371-856d-fbf2e045a950",
        productName: "Tuna",
        productCategory: randomCategory(2),
        quantity: randomQuantity(2)
      },
      {
        productId: "d572c6ac-d112-4ca9-aa9a-48cf2761032c",
        productName: "Pizza",
        productCategory: randomCategory(3),
        quantity: randomQuantity(3)
      }
    ]
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  let response = http.post(url, payload, params);

  check(response, {
    'is status 200': (r) => r.status === 200,
  });

  sleep(1); // Sleep for 1 second between iterations
}

function randomCategory(num) {
  // This function should return a random category based on your requirements
  const categories = ["Electronics", "Books", "Food"];
  return categories[Math.floor(Math.random() * categories.length)];
}

function randomQuantity(num) {
  // This function should return a random quantity between 1 and 10
  return Math.floor(Math.random() * 10) + 1;
}
