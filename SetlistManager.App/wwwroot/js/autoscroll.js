let scrollRequest;
let lastTimestamp;
let preciseScrollY = 0;

window.scrollingFunctions = {
    startAutoScroll: function (containerId, speed) {
        const element = document.getElementById(containerId);
        if (!element) return;

        this.stopAutoScroll();

        preciseScrollY = element.scrollTop;

        const scroll = (timestamp) => {
            if (!lastTimestamp) lastTimestamp = timestamp;
            const elapsed = timestamp - lastTimestamp;

            if (elapsed > 16) {
                const effectiveSpeed = speed / 100;
                preciseScrollY += effectiveSpeed;
                element.scrollTop = preciseScrollY;
                lastTimestamp = timestamp;
            }

            scrollRequest = requestAnimationFrame(scroll);
        };

        scrollRequest = requestAnimationFrame(scroll);
    },
    stopAutoScroll: function () {
        cancelAnimationFrame(scrollRequest);
        lastTimestamp = null;
    }
};