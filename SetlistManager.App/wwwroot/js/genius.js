window.geniusEmbed = {
    loadEmbed: async function (songId, title, artist, url) {
        const container = document.getElementById('genius-lyrics-container');
        if (!container) {
            console.error('Container not found');
            return;
        }

        try {
            const response = await fetch(`https://genius.com/songs/${songId}/embed.js`);
            const scriptText = await response.text();

            const cssMatch = scriptText.match(/href="([^"]+embedded_song[^"]+\.css)"/);
            if (cssMatch && !document.querySelector(`link[href="${cssMatch[1]}"]`)) {
                const link = document.createElement('link');
                link.href = cssMatch[1];
                link.rel = 'stylesheet';
                link.type = 'text/css';
                document.head.appendChild(link);
            }

            const jsonMatch = scriptText.match(/document\.write\(JSON\.parse\(('.*?')\)\)/s);
            if (!jsonMatch) {
                return;
            }

            let jsonString = jsonMatch[1];
            jsonString = jsonString.slice(1, -1);
            jsonString = jsonString.replace(/\\'/g, "'")
                .replace(/\\"/g, '"')
                .replace(/\\\\/g, '\\');

            const htmlContent = JSON.parse(jsonString);

            container.innerHTML = htmlContent;

            const jsMatch = scriptText.match(/src="([^"]+embedded_song[^"]+\.js)"/);
            if (jsMatch && !document.querySelector(`script[src="${jsMatch[1]}"]`)) {
                const script = document.createElement('script');
                script.src = jsMatch[1];
                script.async = true;
                script.crossOrigin = 'true';
                document.body.appendChild(script);
            }

        } catch (error) {
            console.error('Error loading Genius embed:', error);
            container.innerHTML = `
                <div style="padding: 20px; border: 1px solid #ddd; border-radius: 4px; background: #1a1a1a;">
                    <p style="margin: 0 0 10px 0;">View lyrics on <a href="${url}" target="_blank" style="color: #ffff64; font-weight: bold; text-decoration: none;">Genius</a></p>
                    <p style="margin: 0;"><strong style="color: #fff;">${title}</strong> <span style="color: #999;">by ${artist}</span></p>
                </div>
            `;
        }
    }
};