document.addEventListener("DOMContentLoaded", () => {
    const target = document.getElementById("status");
    if (target) {
        const time = new Date().toLocaleTimeString();
        target.innerText = `Hello from script.js! Page loaded at ${time}.`;
    }
});
