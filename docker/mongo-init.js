db = db.getSiblingDB('insurance')

db.createCollection('actors');

db.actors.insertMany([
  {  name: 'John Doe', birthdate: new Date('1990-01-01') },
  {  name: 'Jane Smith', birthdate: new Date('1985-05-15') }
]);