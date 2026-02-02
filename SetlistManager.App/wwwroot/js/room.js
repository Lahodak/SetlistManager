window.scrollToCurrentSong = function () {
    const container = document.getElementById('setlist-container');
    const currentSongCard = document.querySelector('.current-song-card');

    if (container && currentSongCard) {
        container.scrollTop = currentSongCard.offsetTop - container.offsetTop;
    }
}