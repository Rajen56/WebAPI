window.onload = function() {
    const refreshTokenUrl = 'https://yourapi.com/api/login/refresh-token'; // Your refresh token endpoint

    // A helper function to get the stored access token
    function getAccessToken()
    {
        return localStorage.getItem("accessToken");
    }

    // A helper function to set the new access token after refreshing
    function setAccessToken(newToken)
    {
        localStorage.setItem("accessToken", newToken);
    }

    // Modify Swagger's authorization flow
    const ui = SwaggerUIBundle({
    url: "/swagger/v1/swagger.json", // Your Swagger JSON endpoint
        dom_id: '#swagger-ui',
        deepLinking: true,
        presets: [
            SwaggerUIBundle.presets.apis,
            SwaggerUIBundle.SwaggerUIStandalonePreset
        ],
        plugins: [
            SwaggerUIBundle.plugins.DownloadUrl
        ],
        onComplete: function() {
            // Intercept API calls and add Authorization header
            ui.preauthorizeApiKey("Bearer", "Bearer " + getAccessToken());

            // Hook into Swagger's fetch to handle 401 Unauthorized and refresh token
            const originalFetch = window.fetch;
            window.fetch = async function(url, options) {
                const response = await originalFetch(url, options);
                if (response.status === 401)
                {
                    // Token is expired or invalid, so we refresh it
                    const refreshToken = localStorage.getItem("refreshToken");
                    if (refreshToken)
                    {
                        // Make a call to the refresh token endpoint
                        const refreshResponse = await fetch(refreshTokenUrl, {
                        method: 'POST',
                            headers:
                            {
                                'Content-Type': 'application/json',
                            },
                            body: JSON.stringify({ refreshToken: refreshToken })
                        });

                        if (refreshResponse.ok)
                        {
                            const data = await refreshResponse.json();
                            const newAccessToken = data.accessToken;

                            // Store the new access token in localStorage
                            setAccessToken(newAccessToken);

                            // Retry the original request with the new access token
                            options.headers['Authorization'] = 'Bearer ' + newAccessToken;
                            return originalFetch(url, options);
                        }
                        else
                        {
                            // Handle error when refresh token fails
                            window.location.href = "/login"; // Redirect to login
                        }
                    }
                }
                return response;
            };
        }
    });
};