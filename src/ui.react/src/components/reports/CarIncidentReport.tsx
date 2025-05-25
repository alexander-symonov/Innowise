import React, { useState, useEffect } from 'react';

function CarIncidentReport() {

  console.log("render");
  const [state, setState] = useState(() => {
    console.log("initial state");
    return 100;
  });

  useEffect(() => {
    console.log('component mounted');
    return () => {
        console.log('component unmounted');
    };
  }, []);

  useEffect(() => {
    console.log('state changed', state);
    return () => {
        console.log('cleanup state changed', state);
    };
  }, [state]);

  return <div>
    <h1>Car Incident Report</h1>
    <p>State: {state}</p>
    <button onClick={() => setState(state + 1)}>Increment</button>
    <button onClick={() => setState(state - 1)}>Decrement</button>
  </div>
  ;
}

export default React.memo(CarIncidentReport);