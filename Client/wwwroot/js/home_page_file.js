document.addEventListener("DOMContentLoaded", function () {
    // Lấy đối tượng button bằng class name

    //const optionButton = document.querySelector('.js-toggle-chat-options');
    //const mediaButton = document.querySelector('[data-bs-target="#chat-media"]');

    // Bắt sự kiện click vào nút
    //optionButton.addEventListener('click', handleOptionButtonClick);
    //mediaButton.addEventListener('click', handleMediaButtonClick);

    let lastClickedElement = null;

    const items2 = document.querySelectorAll("li.tyn-aside-item.js-toggle-main");
     items2.forEach(addClickEventAndObserver2);

    function addClickEventAndObserver2(item) {
    let isHandling = false;
    
    item.addEventListener("click", () => {
        if (!isHandling && !item.classList.contains("active")) {
            isHandling = true;
            handleActiveClassAdded2(item);
            setTimeout(() => { isHandling = false; }, 0);
        }
    });

    const observer = new MutationObserver(mutations => {
        mutations.forEach(mutation => {
            if (mutation.attributeName === "class") {
                const currentClassState = mutation.target.classList.contains("active");
                if (currentClassState) {
                    handleActiveClassAdded2(mutation.target);
                }
            }
        });
    });

    observer.observe(item, { attributes: true });

}

async function handleActiveClassAdded2(element) {
    if (lastClickedElement !== element) {
        chatUUID = element.id; //bỏ comment ra
        lastClickedElement = element;
        const searchInThisChat = document.querySelector("#searchInThisChat");
        searchInThisChat.value = "";
        currentIndexSearch = 0;
        dictionary = null;

        await handleOptionButtonClick();
        await handleMediaButtonClick();
    }
}
    const containerObserver2 = new MutationObserver(mutations => {
        mutations.forEach(mutation => {
            mutation.addedNodes.forEach(node => {
                if (node.nodeType === Node.ELEMENT_NODE && node.matches("li.tyn-aside-item.js-toggle-main")) {
                    addClickEventAndObserver2(node);
                }
            });
        });
    });

    const containers2 = document.querySelectorAll(".tyn-aside-list");
    containers2.forEach(container => {
        containerObserver2.observe(container, { childList: true, subtree: true });
    });

    async function handleOptionButtonClick(event) {

        await fetchChatMemberId();

        const apiUrl = `${BASE_URL}api/ChatMember/GetChatMember/${chatMemberId}`;
        try {
            const response = await fetch(apiUrl);
            if (!response.ok) throw new Error('Network response was not ok');
            const data = await response.json();
            //console.log('API response:', data);

            updateMuteButton(data.mute);

            await fetchChatDetails(); // Chuyển fetchChatDetails thành async để sử dụng await
            await fetchFriendsInGroup();

        } catch (error) {
            console.error('There was a problem with the fetch operation:', error);
        }
    }

    function updateMuteButton(muteStatus) {
        const button = document.querySelector('.js-chat-mute-toggle');
        button.classList.remove('tyn-chat-mute', 'chat-muted');
        if (muteStatus === 'Off') {
            button.classList.add('tyn-chat-mute');
        } else {
            button.classList.add('tyn-chat-mute', 'chat-muted');
        }
    }

    function handleMediaButtonClick() {

        //await fetchChatMemberId();

        const apiUrl = `${BASE_URL}api/File/GetFileByUUID/${chatUUID}`;
        fetch(apiUrl)
            .then(response => {
                if (!response.ok) throw new Error('Failed to fetch data');
                return response.json();
            })
            .then(data => {
                //console.log('API response:', data);
                renderMedia(data);
            })
            .catch(error => console.error('Error:', error));
    }

    function renderMedia(data) {
        renderFiles(data.filter(file => file.type === 'Document'), 'chat-media-files', renderFileItem);
        renderItems(data.filter(file => file.type === 'Image'), 'chat-media-images', renderImageItem, 'row g-3');
        renderItems(data.filter(file => file.type === 'Video'), 'chat-media-videos', renderVideoItem, 'row g-3');
        refreshLightbox();
    }

    function renderFiles(files, containerId, renderItem) {
        const container = document.getElementById(containerId).querySelector('.tyn-media-list');
        container.innerHTML = '';
        files.forEach(file => container.appendChild(renderItem(file)));
    }

    function renderItems(items, containerId, renderItem, rowClass) {
        const container = document.getElementById(containerId);
        container.innerHTML = '';
        const row = document.createElement('div');
        row.classList.add(...rowClass.split(' '));
        items.forEach(item => row.appendChild(renderItem(item)));
        container.appendChild(row);
    }

    function renderImageItem(file) {
        const col = document.createElement('div');
        col.classList.add('col-4');
        col.innerHTML = `
            <a href="${file.path}" class="glightbox tyn-thumb" data-gallery="media-photo">
                <img src="${file.path}" class="tyn-image" alt="${file.name}">
            </a>`;
        return col;
    }

    function renderVideoItem(file) {
        const col = document.createElement('div');
        col.classList.add('col-6');
        col.innerHTML = `
            <a href="${file.path}" class="glightbox tyn-video" data-gallery="media-video">
                <img src="${file.path}" class="tyn-image" alt="${file.name}">
                <div class="tyn-video-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-play-fill" viewBox="0 0 16 16">
                        <path d="m11.596 8.697-6.363 3.692c-.54.313-1.233-.066-1.233-.697V4.308c0-.63.692-1.01 1.233-.696l6.363 3.692a.802.802 0 0 1 0 1.393z"/>
                    </svg>
                </div>
            </a>`;
        return col;
    }

    function renderFileItem(file) {
        const li = document.createElement('li');
        const iconClass = getFileTypeIcon(file.extension);
        li.innerHTML = `
            <a href="${file.path}" class="tyn-file">
                <div class="tyn-media-group">
                    <div class="tyn-media tyn-size-lg text-bg-light">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi ${iconClass}" viewBox="0 0 16 16">
                            <path fill-rule="evenodd" d="${file.iconPath}"/>
                        </svg>
                    </div>
                    <div class="tyn-media-col">
                        <h6 class="name">${file.name}</h6>
                        <div class="meta">${file.size}</div>
                    </div>
                </div>
            </a>`;
        return li;
    }

    function updateAvatar(newImageSrc, newName) {
        const avatarImg = document.getElementById("avatar-img");
        const avatarName = document.getElementById("avatar-name");

        if (avatarImg && avatarName) {
            avatarImg.src = newImageSrc;
            avatarName.textContent = newName;
        } else {
            console.error("Could not find the avatar image or name element.");
        }
    }

    function updateFriendsInfo(friendsInfo) {

        // Chọn phần tử div chứa
        const container = document.getElementById("chat-options-member");

        if (!container) {
            console.error("Could not find the container element.");
            return;
        }

        // Chọn phần tử ul với lớp tyn-media-list trong phần tử chứa
        const friendsList = container.querySelector(".tyn-media-list");

        if (!friendsList) {
            console.error("Could not find the friends list element within the specified container.");
            return;
        }

        // Xóa các mục hiện có trong danh sách (nếu cần)
        friendsList.innerHTML = '';

        friendsInfo.forEach(friend => {
            const listItem = document.createElement("li");

            listItem.innerHTML = `
            <a href="#" class="tyn-file">
                <div class="tyn-media-group">
                    <div class="tyn-media text-bg-light">
                        <img src="${friend.avatarurl}" width="16" height="16" alt="Friend's Avatar" class="bi bi-person-x-fill" />
                    </div>
                    <div class="tyn-media-col">
                        <h6 class="name">${friend.chatname}</h6>
                        <div class="meta">Active</div>
                    </div>
                </div>
            </a>
        `;

            friendsList.appendChild(listItem);
        });
    }

    async function fetchChatMemberId() {
        const token = await getToken(); // Giả sử getToken() là hàm để lấy token

        if (!token) {
            console.error("Token is required to fetch chat details.");
            return;
        }

        const url = `${BASE_URL}api/ChatMember/GetChatMemberId/${chatUUID}`;

        try {
            const response = await fetch(url, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`, // Thêm tiêu đề Authorization
                },
            });

            if (!response.ok) {
                console.error(
                    "Network response was not ok",
                    response.status,
                    response.statusText
                );
                throw new Error("Network response was not ok " + response.statusText);
            }

            const data = await response.json();

            chatMemberId = data;

            return chatMemberId;

        } catch (error) {
            console.error("There was a problem with the fetch operation:", error);
            throw error;
        }
    }

    async function fetchChatDetails() {
        const token = await getToken(); // Giả sử getToken() là hàm để lấy token

        if (!token) {
            console.error("Token is required to fetch chat details.");
            return;
        }

        const url = `${BASE_URL}api/Chat/chats_user/${chatUUID}`;

        try {
            const response = await fetch(url, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`, // Thêm tiêu đề Authorization
                },
            });

            if (!response.ok) {
                console.error(
                    "Network response was not ok",
                    response.status,
                    response.statusText
                );
                throw new Error("Network response was not ok " + response.statusText);
            }

            const chat = await response.json();

            // Xử lý và gán avatarUrl và chatName dựa trên dữ liệu chat
            const avatarUrl = chat.isGroup ? chat.avatarUrl : chat.otherUser.avatarUrl;
            const chatName = chat.isGroup ? chat.chatName : chat.otherUser.nickName;

            updateAvatar(avatarUrl, chatName);

            return { avatarUrl, chatName };

        } catch (error) {
            console.error("There was a problem with the fetch operation:", error);
            throw error; // Đẩy lỗi ra ngoài nếu cần
        }
    }

    async function fetchFriendsInGroup() {
        const token = await getToken(); // Giả sử getToken() là hàm để lấy token

        if (!token) {
            console.error("Token is required to fetch chat details.");
            return;
        }

        const url = `${BASE_URL}api/Profile/friends_in_group/${chatUUID}`;

        try {
            const response = await fetch(url, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`, // Thêm tiêu đề Authorization
                },
            });

            if (!response.ok) {
                console.error(
                    "Network response was not ok",
                    response.status,
                    response.statusText
                );
                throw new Error("Network response was not ok " + response.statusText);
            }

            const data = await response.json();

            const friendsinfo = data.map(profile => ({
                avatarurl: profile.avatarUrl,
                chatname: profile.nickName
            }));

            if (friendsinfo.length < 2) {
                setupHideTabOnClick(true);
            } else {
                setupHideTabOnClick(false);
            }

            updateFriendsInfo(friendsinfo);

            return { friendsInfo };

        } catch (error) {
            console.error("There was a problem with the fetch operation:", error);
            throw error; // Đẩy lỗi ra ngoài nếu cần
        }
    }

    function setupHideTabOnClick(shouldHide) {
        const membersTabItem = document.getElementById('members-tab-item');

        if (membersTabItem) {
            if (shouldHide) {
                membersTabItem.style.display = 'none'; 
            } else {
                membersTabItem.style.display = 'block'; 
            }
        }
    }



    function getFileTypeIcon(extension) {
        const iconMap = {
            docx: 'filetype-docx',
            ai: 'filetype-ai',
            pdf: 'filetype-pdf',
            csv: 'filetype-csv',
            mdx: 'filetype-mdx',
        };
        return iconMap[extension] || 'filetype-docx';
    }

    function refreshLightbox() {
        GLightbox({ selector: '.glightbox' });
    }
});
