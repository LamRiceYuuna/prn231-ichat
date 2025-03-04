document.addEventListener("DOMContentLoaded", () => {
    const dropdownNotification = document.querySelector("#dropdown-notification");
    const numberNotification = dropdownNotification.querySelector("#number-notification");
    let simpleNotification = new SimpleBar(document.querySelector('#scroll-notification'));

    const observer = new MutationObserver((mutationsList) => {
        for (const mutation of mutationsList) {
            if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                if (dropdownNotification.classList.contains('show')) {
                    handlerDropdownNotification();
                } else {
                    console.log('Class "show" đã bị gỡ bỏ.');
                }
            }
        }
    });
    observer.observe(dropdownNotification, { attributes: true });
        
    function handlerDropdownNotification() {
        
        //Call api or signalR chuyển notification thành read
        connection.invoke("NoticeSeen", mapNotification)
            .then((status) => {
                if (status) {
                    mapNotification = new Map();
                    numberNotification.textContent = "";
                    console.log(status);
                }
            })
            .catch(err => {
                console.error(err.toString());
            });
    }

    loadNotification();
    const containerNotification = document.querySelector("#container-notification");
    var mapNotification = new Map();

    connection.on("ReceiveNotifications", (notification) => {
        addNotification(notification);
        numberNotification.textContent = `${Number(numberNotification.textContent) + 1}`;
        console.log(notification)
    });
    
    async function loadNotification() {
        const url = `${BASE_URL}api/Notification/load`
        var rs = await getDataFromUrl(url);
        rs.forEach(notification => {
            addNotification(notification);
        });
        numberNotification.textContent = mapNotification.size;
        if (mapNotification.size == 0) {
            numberNotification.textContent = ""
        }
    }
    function addNotification(notification) {
        if (notification.status == "Unread") {
            mapNotification.set(notification.notificationId, notification.notificationFriendResponse.userSendRequest)
        }
        var noti = createNotificationMessage(notification.notificationFriendResponse);
        containerNotification.insertAdjacentHTML("beforeend", noti);
    }

    // properties: avatarUrl, name, content, dateAgo
    function createNotificationMessage(notification) {
        return `
            <li>
                <div class="tyn-media-group">
                    <div class="tyn-media tyn-circle">
                        <img src="${BASE_URL}api/File/image/${notification.avatarUrl}" alt="">
                    </div>
                    <div class="tyn-media-col">
                        <div class="tyn-media-row">
                            <span class="message"><strong>${notification.name}</strong> ${notification.content}</span>
                        </div>
                        <div class="tyn-media-row has-dot-sap">
                            <span class="meta">${formatDateTime(notification.dateAgo)}</span>
                        </div>
                    </div>
                </div><!-- .tyn-media-group -->
            </li><!-- li -->
        `
    }
    function formatDateTime(dateString) {
        let date = new Date(dateString);

        if (isNaN(date.getTime())) {
            throw new Error("Invalid date format");
        }

        let day = String(date.getDate()).padStart(2, '0');
        let month = String(date.getMonth() + 1).padStart(2, '0')
        let year = date.getFullYear();

        let hours = String(date.getHours()).padStart(2, '0');
        let minutes = String(date.getMinutes()).padStart(2, '0');
        let seconds = String(date.getSeconds()).padStart(2, '0');

        return `${day}/${month}/${year} ${hours}:${minutes}:${seconds}`;
    }

    async function getDataFromUrl(url) {
        const token = await getToken();

        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const data = await response.json();
            console.log('Response data:', data);
            return data;
        } catch (error) {
            console.error('Fetch error:', error);
        }
    }
});