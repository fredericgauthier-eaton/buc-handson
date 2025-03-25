const request = require("supertest");

let createApp = require('./index'); // Import the app factory function

describe('People API (stateless simulation)', () => {
    let app1, app2;

    beforeEach(() => {
        app1 = createApp(); // Create a new instance of the app
        app2 = createApp(); // Create another new instance of the app
    });

    it("Simulation of 2 instances, test that the microservice is stateless", async () => {
        let persons = [
            { id: 0, name: 'Luiz', age: 32 },
            { id: 1, name: 'Peter', age: 26 }
        ];

        let res = await request(app1)
            .get("/people")
            .set('username', 'calaca')
            .set('password', '12345')
            .expect(200);

        expect(res.body).toEqual(persons);

        const newPerson = { name: 'Alice', age: 25 };

        res = await request(app1)
            .post("/people")
            .send(newPerson) // Pass newPerson as the request body
            .set('username', 'calaca')
            .set('password', '12345')
            .expect(201); // Expect 201 for successful creation

        newPerson.id = 2;
        persons.push(newPerson);

        res = await request(app2)
            .get("/people")
            .set('username', 'calaca')
            .set('password', '12345')
            .expect(200);
        expect(res.body).toEqual(persons); // app2 should not reflect changes made in app1*/

        
       /* res = await request(app2)
            .get("/people")
            .set('username', 'calaca')
            .set('password', '12345')
            .expect(200);
        expect(res.body).toEqual(persons); // app2 should not reflect changes made in app1*/
    });
});
