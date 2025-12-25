window.geniusEmbed = {
    loadEmbed: function (songId, title, artist, url) {
        const container = document.getElementById('genius-lyrics-container');
        if (!container) {
            console.error('Container not found');
            return;
        }

        container.innerHTML = `
            <div style="border: 1px solid #e0e0e0; border-radius: 8px; padding: 24px; background: #fafafa; max-width: 600px;">
                <div style="margin-bottom: 16px; padding-bottom: 12px; border-bottom: 1px solid #e0e0e0;">
                    <span style="color: #999; text-decoration: none; font-size: 11px; font-weight: 600; letter-spacing: 0.5px;">
                        POWERED BY GENIUS
                    </span>
                </div>
                <div style="margin-bottom: 20px;">
                    <div style="font-size: 20px; font-weight: 600; margin-bottom: 8px; color: #000; line-height: 1.3;">
                        ${title}
                    </div>
                    <div style="font-size: 15px; color: #666;">
                        ${artist}
                    </div>
                </div>
                <div>
                    <a href="${url}" 
                       target="_blank" 
                       style="display: inline-block; padding: 12px 24px; background: #ffff64; color: #000; text-decoration: none; border-radius: 4px; font-weight: 600; font-size: 14px; transition: background 0.2s;"
                       onmouseover="this.style.background='#ffff50'" 
                       onmouseout="this.style.background='#ffff64'">
                        Read Full Lyrics on Genius →
                    </a>
                </div>
            </div>
        `;
    }
};