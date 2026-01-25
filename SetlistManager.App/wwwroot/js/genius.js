window.geniusEmbed = {
    loadEmbed: async function (songId, title, artist, url) {
        const container = document.getElementById('genius-lyrics-container');
        if (!container) {
            console.error('Container not found');
            return;
        }

        try {
            // Fetch the embed script
            const response = await fetch(`https://genius.com/songs/${songId}/embed.js`);
            const scriptText = await response.text();

            // Extract the CSS link
            const cssMatch = scriptText.match(/href="([^"]+embedded_song[^"]+\.css)"/);
            if (cssMatch && !document.querySelector(`link[href="${cssMatch[1]}"]`)) {
                const link = document.createElement('link');
                link.href = cssMatch[1];
                link.rel = 'stylesheet';
                link.type = 'text/css';
                document.head.appendChild(link);
            }

            // Extract the JSON string more carefully
            const jsonMatch = scriptText.match(/document\.write\(JSON\.parse\(('.*?')\)\)/s);
            if (!jsonMatch) {
                return;
            }

            // Parse the outer single-quoted string, then parse the JSON inside
            let jsonString = jsonMatch[1];
            // Remove outer quotes
            jsonString = jsonString.slice(1, -1);
            // Unescape the string
            jsonString = jsonString.replace(/\\'/g, "'")
                .replace(/\\"/g, '"')
                .replace(/\\\\/g, '\\');

            // Now parse as JSON
            const htmlContent = JSON.parse(jsonString);

            // Inject the HTML
            container.innerHTML = htmlContent;

            // Load the interactive script
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
            // Fallback: simple link
            container.innerHTML = `
                <div style="padding: 20px; border: 1px solid #ddd; border-radius: 4px; background: #1a1a1a;">
                    <p style="margin: 0 0 10px 0;">View lyrics on <a href="${url}" target="_blank" style="color: #ffff64; font-weight: bold; text-decoration: none;">Genius</a></p>
                    <p style="margin: 0;"><strong style="color: #fff;">${title}</strong> <span style="color: #999;">by ${artist}</span></p>
                </div>
            `;
        }
    }
};