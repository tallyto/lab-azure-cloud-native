const NUMBERS = [
    0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26
];

const RED_NUMBERS = [1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36];

function initializeWheel() {
    const wheel = document.getElementById('roulette-wheel');
    
    // Add outer and inner rings
    const outerRing = document.createElement('div');
    outerRing.className = 'wheel-outer-ring';
    wheel.appendChild(outerRing);
    
    const innerRing = document.createElement('div');
    innerRing.className = 'wheel-inner-ring';
    wheel.appendChild(innerRing);
    
    // Create number pockets
    NUMBERS.forEach((number, index) => {
        const container = document.createElement('div');
        container.className = 'number-container';
        const angle = (index * 360 / NUMBERS.length);
        container.style.transform = `rotate(${angle}deg)`;
        
        const numberDiv = document.createElement('div');
        numberDiv.className = `number ${number === 0 ? 'green' : RED_NUMBERS.includes(number) ? 'red' : 'black'}`;
        numberDiv.textContent = number;
        
        container.appendChild(numberDiv);
        wheel.appendChild(container);
    });
}

let isSpinning = false;
let currentRotation = 0;

function calculateBallPosition(number) {
    const index = NUMBERS.indexOf(number);
    return (index * (360 / NUMBERS.length)) + (360 / NUMBERS.length / 2);
}

function spinWheel() {
    if (isSpinning) return;
    
    const wheel = document.getElementById('roulette-wheel');
    const ball = document.getElementById('ball');
    const result = document.getElementById('result');
    const spinButton = document.getElementById('spin-button');
    const wheelSound = document.getElementById('wheelSound');
    const ballSound = document.getElementById('ballSound');
    const winSound = document.getElementById('winSound');

    // Remove previous highlights
    document.querySelectorAll('.number.highlight').forEach(el => el.classList.remove('highlight'));

    isSpinning = true;
    spinButton.disabled = true;
    result.textContent = 'Spinning...';

    // Add spinning class for CSS animation
    wheel.classList.add('spinning');

    // Start sounds
    wheelSound.currentTime = 0;
    ballSound.currentTime = 0;
    wheelSound.play();
    ballSound.play();

    const spins = 8 + Math.random() * 4;
    const extraDegrees = Math.random() * 360;
    const totalRotation = spins * 360 + extraDegrees;
    currentRotation += totalRotation;

    wheel.style.transform = `rotate(${currentRotation}deg)`;

    // Enhanced ball animation
    // Slow down the initial ball spin
    ball.style.transition = 'transform 1.2s linear';
    ball.classList.add('spin-ball');

    setTimeout(() => {
        // Slow down sounds
        const fadeOutInterval = setInterval(() => {
            wheelSound.volume = Math.max(0, wheelSound.volume - 0.1);
            ballSound.volume = Math.max(0, ballSound.volume - 0.1);
            if (wheelSound.volume <= 0) {
                clearInterval(fadeOutInterval);
                wheelSound.pause();
                ballSound.pause();
            }
        }, 200);

        ball.classList.remove('spin-ball');
        // Calculate winning number more robustly
        const normalizedRotation = (currentRotation % 360 + 360) % 360;
        const sectorSize = 360 / NUMBERS.length;
        let index = Math.floor((360 - normalizedRotation + sectorSize / 2) % 360 / sectorSize);
        if (index < 0) index += NUMBERS.length;
        const winningNumber = NUMBERS[index];
        const finalBallPosition = calculateBallPosition(winningNumber);

        // Slow down the final ball movement
        ball.style.transition = 'transform 2.5s cubic-bezier(0.25, 0.46, 0.45, 0.94)';
        ball.style.transform = `translate(-50%, 0) rotate(${finalBallPosition}deg)`;

        // Highlight the winning number
        document.querySelectorAll('.number').forEach(el => {
            if (parseInt(el.textContent) === winningNumber) {
                el.classList.add('highlight');
            }
        });

        // Remove spinning class
        wheel.classList.remove('spinning');

        isSpinning = false;
        spinButton.disabled = false;

        // Play win sound
        winSound.currentTime = 0;
        winSound.play();

        result.textContent = `Winner: ${winningNumber}!`;
        result.classList.add('winner-animation');

        setTimeout(() => result.classList.remove('winner-animation'), 1000);
    }, 5000);
}

// Reset sound volumes on page load
window.addEventListener('load', () => {
    initializeWheel();
    document.getElementById('spin-button').addEventListener('click', spinWheel);
    const sounds = document.getElementsByTagName('audio');
    for (let sound of sounds) {
        sound.volume = 1.0;
    }
    // Optional: fade-in effect for the wheel
    const wheel = document.getElementById('roulette-wheel');
    if (wheel) {
        wheel.style.opacity = 0;
        setTimeout(() => { wheel.style.transition = 'opacity 1s'; wheel.style.opacity = 1; }, 100);
    }
});