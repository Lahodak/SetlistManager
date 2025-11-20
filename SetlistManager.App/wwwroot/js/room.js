window.scrollToCurrentSong = function () {
    const container = document.getElementById('setlist-container');
    const currentSongCard = document.querySelector('.current-song-card');

    if (container && currentSongCard) {
        // Scroll the current song to the top of the container
        container.scrollTop = currentSongCard.offsetTop - container.offsetTop;
    }
}