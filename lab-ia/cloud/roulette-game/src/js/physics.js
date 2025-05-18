function calculateSpinVelocity(initialVelocity, friction, time) {
    return initialVelocity * Math.exp(-friction * time);
}

function calculateBallPosition(initialPosition, velocity, time) {
    return initialPosition + velocity * time + 0.5 * gravity * time * time;
}

const gravity = 9.81; // Acceleration due to gravity in m/s^2

function updatePhysics(spinTime, initialBallPosition, initialSpinVelocity, friction) {
    const currentSpinVelocity = calculateSpinVelocity(initialSpinVelocity, friction, spinTime);
    const currentBallPosition = calculateBallPosition(initialBallPosition, currentSpinVelocity, spinTime);
    
    return {
        spinVelocity: currentSpinVelocity,
        ballPosition: currentBallPosition
    };
}