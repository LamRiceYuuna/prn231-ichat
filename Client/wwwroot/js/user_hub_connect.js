async function getToken() {
    const token = getCookie('bearer_token');
    if (!token) {
        console.error("No token found in cookies");
        if (window.location.pathname !== "/") {
            window.location.href = "/";
        }
        return null;
    }
    return token;
}
function parseJwt(token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));

    return JSON.parse(jsonPayload);
}

async function getUser() {
    var token = await getToken();
    if (token){
        var user = parseJwt(token);
        return user;
    }
    return null;
}
function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}
var connection;
async function startSignalRConnection() {
    try {
        const token = await getToken();

        if (!token) {
            console.error("Token is required to establish SignalR connection.");
            return;
        }

        connection = new signalR.HubConnectionBuilder()
            .withUrl(BASE_URL + "Ichat/User", {
                accessTokenFactory: () => token
            })
            .build();

        connection.onclose(async (error) => {
            if (error) {
                console.error("Connection closed with error:", error);
                if (error.statusCode === 401) {
                    console.error("Unauthorized access - removing token");
                    document.cookie = "bearer_token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
                    window.location.href = "/";
                } else {
                    await startSignalRConnection();
                }
            }
        });

        await connection.start();
        console.log("SignalR Connected.");

    } catch (err) {
        console.error("Error establishing SignalR connection: ", err);
        if (err.statusCode === 401) {
            console.error("Unauthorized access - removing token");
            localStorage.removeItem("bearer_token");
            window.location.href = "/";
        }
    }
}

document.addEventListener("DOMContentLoaded", startSignalRConnection);
const BASE_URL = "https://localhost:7119/";
