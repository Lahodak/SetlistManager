export function toggleFullscreen() {
    if (!document.fullscreenElement) {
        document.documentElement.requestFullscreen().catch(err => {
            console.error(`Error attempting to enable fullscreen: ${err.message}`);
        });
        return true;
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        }
        return false;
    }
}

export function isFullscreen() {
    return !!document.fullscreenElement;
}

document.addEventListener('fullscreenchange', () => {
    const isFullscreen = !!document.fullscreenElement;
    window.dispatchEvent(new CustomEvent('fullscreenchanged', { detail: isFullscreen }));
});