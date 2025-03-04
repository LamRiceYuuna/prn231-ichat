document.addEventListener('DOMContentLoaded', async function () {
    reloadTabAll();
    loadInfoCurrentUser();
});

document.addEventListener('DOMContentLoaded',async function () {
    
    const btAllButton = document.getElementById('btnAll');

    btAllButton.addEventListener('click', function () {
        reloadTabAll();
    });
});

async function reloadTabAll() {
    const token = await getToken();
    const response = await fetch(`${BASE_URL}api/User/friends`, {
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    });
    const data = await response.json();
    const asideList = document.querySelector('#con-all');
    const searchInput = document.querySelector('#search-main');


    function renderFriendList(friends) {
        asideList.innerHTML = ''; 
        friends.forEach(friend => {
            const listItem = createFriendListItem(friend);
            listItem.addEventListener('click', async function () {
                await handleListItemClick(friend, listItem);
            });
            asideList.appendChild(listItem);
        });
    }

    renderFriendList(data);

    searchInput.addEventListener('input', function () {
      
        const searchTerm = searchInput.value.toLowerCase();
        const filteredFriends = data.filter(friend =>
            friend.nickName.toLowerCase().includes(searchTerm)
            
        );
        renderFriendList(filteredFriends);
    });
}


function createFriendListItem(friend) {
    const listItem = document.createElement('li');
    listItem.classList.add('tyn-aside-item', 'js-toggle-main');
    listItem.dataset.friendId = friend.uuid;
    listItem.innerHTML = `
        <div class="tyn-media-group">
            <div class="tyn-media tyn-size-lg">
                <img src="${friend.avatarUrl}" alt="">
            </div>
            <div class="tyn-media-col">
                <div class="tyn-media-row">
                    <h6 class="name">${friend.nickName}</h6>
                </div>
                <div class="tyn-media-row">
                    <p class="content">@${friend.userName}</p>
                </div>
            </div>
            <div class="tyn-media-option tyn-aside-item-option">
                <ul class="tyn-media-option-list">
                    <li class="dropdown">
                        <button class="btn btn-icon btn-white btn-pill dropdown-toggle" data-bs-toggle="dropdown" data-bs-offset="0,0">
                            <!-- three-dots -->
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-three-dots" viewBox="0 0 16 16">
                                <path d="M3 9.5a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3z" />
                            </svg>
                        </button>
                        <div class="dropdown-menu dropdown-menu-end">
                            <ul class="tyn-list-links">
                                <li>
                                    <a href="#" class="send-message" data-uuid="${friend.uuid}">
                                        <!-- chat -->
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chat" viewBox="0 0 16 16">
                                            <path d="M2.678 11.894a1 1 0 0 1 .287.801 10.97 10.97 0 0 1-.398 2c1.395-.323 2.247-.697 2.634-.893a1 1 0 0 1 .71-.074A8.06 8.06 0 0 0 8 14c3.996 0 7-2.807 7-6 0-3.192-3.004-6-7-6S1 4.808 1 8c0 1.468.617 2.83 1.678 3.894zm-.493 3.905a21.682 21.682 0 0 1-.713.129c-.2.032-.352-.176-.273-.362a9.68 9.68 0 0 0 .244-.637l.003-.01c.248-.72.45-1.548.524-2.319C.743 11.37 0 9.76 0 8c0-3.866 3.582-7 8-7s8 3.134 8 7-3.582 7-8 7a9.06 9.06 0 0 1-2.347-.306c-.52.263-1.639.742-3.468 1.105z" />
                                        </svg>
                                        <span>Send Message</span>
                                    </a>
                                </li>
                                <li class="dropdown-divider"></li>
                                <li>
                                    <a href="#" class="block-user" data-uuid="${friend.uuid}">
                                        <!-- person-x -->
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-person-x" viewBox="0 0 16 16">
                                            <path d="M11 5a3 3 0 1 1-6 0 3 3 0 0 1 6 0ZM8 7a2 2 0 1 0 0-4 2 2 0 0 0 0 4Zm.256 7a4.474 4.474 0 0 1-.229-1.004H3c.001-.246.154-.986.832-1.664C4.484 10.68 5.711 10 8 10c.26 0 .507.009.74.025.226-.341.496-.65.804-.918C9.077 9.038 8.564 9 8 9c-5 0-6 3-6 4s1 1 1 1h5.256Z" />
                                            <path d="M12.5 16a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7Zm-.646-4.854.646.647.646-.647a.5.5 0 0 1 .708.708l-.647.646.647.646a.5.5 0 0 1-.708.708l-.646-.647-.646.647a.5.5 0 0 1-.708-.708l.647-.646-.647-.646a.5.5 0 0 1 .708-.708Z" />
                                        </svg>
                                        <span>Block</span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <!-- exclamation-triangle -->
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-exclamation-triangle" viewBox="0 0 16 16">
                                            <path d="M7.938 2.016A.13.13 0 0 1 8.002 2a.13.13 0 0 1 .063.016.146.146 0 0 1 .054.057l6.857 11.667c.036.06.035.124.002.183a.163.163 0 0 1-.054.06.116.116 0 0 1-.066.017H1.146a.115.115 0 0 1-.066-.017.163.163 0 0 1-.054-.06.176.176 0 0 1 .002-.183L7.884 2.073a.147.147 0 0 1 .054-.057zm1.044-.45a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566z" />
                                            <path d="M7.002 12a1 1 0 1 1 2 0 1 1 0 0 1-2 0zM7.1 5.995a.905.905 0 1 1 1.8 0l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995z" />
                                        </svg>
                                        <span>Report</span>
                                    </a>
                                </li>
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
    `;

    // Thêm sự kiện click cho nút Block
    listItem.querySelector('.block-user').addEventListener('click', async function (event) {
        event.preventDefault();
        const uuid = this.dataset.uuid;
        const token = await getToken();

        try {
            const response = await fetch(`https://localhost:7119/api/Block/Blocked/${uuid}`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {               
                listItem.remove();
            } else {
                const errorMessage = await response.text();
                throw new Error(errorMessage);
            }
        } catch (error) {
            console.error('Error blocking user:', error.message);
        }
    });

    // Thêm sự kiện click cho nút Send Message
    listItem.querySelector('.send-message').addEventListener('click',async function (event) {
        event.preventDefault();
        const uuid = this.dataset.uuid;
        var user = await getUser();
        window.location.href = `/Chat/HomeChat?uuidU=${uuid}&uuidF=${user.uuid}`;
    });

    return listItem;
}
////////////////
document.addEventListener('DOMContentLoaded', async function () {
    reloadTabRequest();

    const btAllButton = document.getElementById('btnRequest');

    btAllButton.addEventListener('click', function () {
        reloadTabRequest();
    });
});

async function reloadTabRequest() {
    const token = await getToken();
    const response = await fetch(`${BASE_URL}api/User/FriendRequest`, {
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    });
    const data = await response.json();
    const asideList = document.querySelector('#con-request');
    const searchInput = document.querySelector('#search-main');

    // Hàm để tạo danh sách bạn bè
    function renderFriendList(friends) {
        asideList.innerHTML = ''; // Xóa nội dung cũ
        friends.forEach(friend => {
            const listItem = createFriendRequest(friend);
            listItem.addEventListener('click', async function () {
                await handleListItemClick(friend, listItem);
            });
            asideList.appendChild(listItem);
        });
    }

    // Hiển thị toàn bộ danh sách bạn bè khi tải trang
    renderFriendList(data);

    // Sự kiện tìm kiếm trực tiếp
    searchInput.addEventListener('input', function () {
        const searchTerm = searchInput.value.toLowerCase();
        const filteredFriends = data.filter(friend =>
            friend.nickName.toLowerCase().includes(searchTerm)
        );
        renderFriendList(filteredFriends);
    });
}

function createFriendRequest(friend) {
    const listItem = document.createElement('li');
    listItem.classList.add('tyn-aside-item', 'js-toggle-main');
    listItem.dataset.friendId = friend.uuid;
    listItem.innerHTML = `
        <div class="tyn-media-group">
            <div class="tyn-media tyn-size-lg">
                <img src="${friend.avatarUrl}" alt="">
            </div>
            <div class="tyn-media-col">
                <div class="tyn-media-row">
                    <h6 class="name">${friend.nickName}</h6>
                </div>
                <div class="tyn-media-row">
                    <p class="content">@${friend.userName}</p>
                </div>
            </div>
            <div class="tyn-media-option tyn-aside-item-option">
                <ul class="tyn-media-option-list">
                    <li class="dropdown">
                        <button class="btn btn-icon btn-white btn-pill dropdown-toggle" data-bs-toggle="dropdown" data-bs-offset="0,0">
                            <!-- three-dots -->
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-three-dots" viewBox="0 0 16 16">
                                <path d="M3 9.5a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3z" />
                            </svg>
                        </button>
                        <div class="dropdown-menu dropdown-menu-end">
                            <ul class="tyn-list-links">
                                <li>
                                    <button class="accept-btn" data-uuid="${friend.uuid}">
                                        <span>Accept</span>
                                    </button>
                                </li>
                                <li class="dropdown-divider"></li>
                                <li>       
                                    <button class="reject-btn" data-uuid="${friend.uuid}">
                                        <span>Reject</span>
                                    </button>
                                </li>                               
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
    `;

    // Thêm sự kiện click cho nút Accept
    listItem.querySelector('.accept-btn').addEventListener('click', async function () {
        await updateFriendStatus(friend.uuid, 1);       
        loadFriendRequests();
    });

    // Thêm sự kiện click cho nút Reject
    listItem.querySelector('.reject-btn').addEventListener('click', async function () {
        await updateFriendStatus(friend.uuid, 2);      
        loadFriendRequests();
    });
    return listItem;
}

async function updateFriendStatus(friendUuid, code) {
    const token = await getToken();
    const user = await getUser();
    const url = `${BASE_URL}api/Friend/UpdateStatusFriendShip/${selectedFriendUUID}/${user.uuid}/${code}`;

    try {
        const response = await fetch(url, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            const errorMessage = await response.text();
            throw new Error(errorMessage);
        }

        console.log('Friend status updated successfully.');
    } catch (error) {
        console.error('Error updating friend status:', error.message);
    }
}

async function loadFriendRequests() {
    const token = await getToken();
    const asideList = document.querySelector('#con-request');
    try {
        const response = await fetch(`${BASE_URL}api/User/FriendRequest`, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const data = await response.json();

            // Hàm để tạo danh sách bạn bè
            function renderFriendList(friends) {
                asideList.innerHTML = ''; // Xóa nội dung cũ
                friends.forEach(friend => {
                    const listItem = createFriendRequest(friend);
                    listItem.addEventListener('click', async function () {
                        await handleListItemClick(friend, listItem);
                    });
                    asideList.appendChild(listItem);
                });
            }

            // Hiển thị toàn bộ danh sách bạn bè khi tải trang
            renderFriendList(data);
        } else if (response.status === 404) {
            asideList.innerHTML = '';
        } else {
            asideList.innerHTML = '';
        }
    } catch (error) {
        console.error('Error fetching friend requests:', error);
        asideList.innerHTML = '';
    }
}

async function loadInfoCurrentUser() {
    var user = await getUser();
    let url = `${BASE_URL}api/User/`;
    const response = await fetch(url + user.uuid);
    const friendDetail = await response.json();
    selectedFriendUUID = user.uuid;
    
    updateFriendDetail(friendDetail);

    const galleryButton = document.querySelector('#gallery-click');
        if (galleryButton.classList.contains('active')) {
            await loadGallery(selectedFriendUUID);           
        }

        const storiesButton = document.querySelector('#stories-click');
        if (storiesButton.classList.contains('active')) {
            await loadStories(selectedFriendUUID);
        }

        const mutualButton = document.querySelector('#mutual-click');
        if (mutualButton.classList.contains('active')) {
            await loadMutual(selectedFriendUUID);
        }
    
}



async function handleListItemClick(friend, listItem) {
    
    selectedFriendUUID = friend.uuid;  
    document.querySelectorAll('.tyn-aside-item').forEach(item => {
        item.classList.remove('active');
    });

    if (listItem != null) {

        listItem.classList.add('active');
    }
    
    try {
        let url = `${BASE_URL}api/User/`;
        const response = await fetch(url + selectedFriendUUID);
        const friendDetail = await response.json();

        updateFriendDetail(friendDetail);

        const galleryButton = document.querySelector('#gallery-click');
        if (galleryButton.classList.contains('active')) {
            await loadGallery(selectedFriendUUID);           
        }

        const storiesButton = document.querySelector('#stories-click');
        if (storiesButton.classList.contains('active')) {
            await loadStories(selectedFriendUUID);
        }

        const mutualButton = document.querySelector('#mutual-click');
        if (mutualButton.classList.contains('active')) {
            await loadMutual(selectedFriendUUID);
        }
    } catch (error) {
        console.error('Error fetching friend detail:', error);
    }
}

getU();
async function getU() {
    var user = await getUser();
}

function updateFriendDetail(friendDetail) {
   
    const userInfoDetails = document.getElementById('user-info-details');
   
    userInfoDetails.innerHTML = `
        <div class="tyn-media-group align-items-start">
            <div class="tyn-media tyn-media-bordered tyn-size-4xl tyn-profile-avatar">
                <img src="${friendDetail.avatarUrl}" alt="Avatar">
            </div>
            <div class="tyn-media-col">
                <div class="tyn-media-row">
                    <h4 class="name">${friendDetail.nickName} <span class="username">@${friendDetail.userName}</span></h4>
                </div>
                <div class="tyn-media-row has-dot-sap">
                    <span class="content">Wellcome</span>
                    <span class="meta">IChat</span>
                </div>
                <div class="tyn-media-row pt-2">
                                            <div class="tyn-media-multiple">
                                                
                                            </div>
                                        </div>
            </div>
        </div>`;
}


document.addEventListener('DOMContentLoaded', async function () {
    const token = await getToken();
    const response = await fetch(`${BASE_URL}api/User/getuser`, {
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    });
    const data = await response.json();
    const asideList = document.querySelector('#con-new');
    const searchInput = document.querySelector('#search-main');
    const threeDaysAgo = new Date();
    threeDaysAgo.setDate(threeDaysAgo.getDate() - 3);

    // Hàm để tạo danh sách bạn bè
    function renderFriendList(friends) {
        asideList.innerHTML = ''; // Xóa nội dung cũ
        friends.forEach(friend => {
            const createdAtDate = new Date(friend.createdAt);
            if (createdAtDate >= threeDaysAgo) {
                const listItem = createFriendNewList(friend);
                listItem.addEventListener('click', async function () {
                    await handleListItemClick(friend, listItem);
                });
                asideList.appendChild(listItem);
            }
        });
    }

    renderFriendList(data.friendships);

    searchInput.addEventListener('input', function () {
        const searchTerm = searchInput.value.toLowerCase();
        const filteredFriends = data.friendships.filter(friend =>
            friend.nickName.toLowerCase().includes(searchTerm) ||
            friend.username.toLowerCase().includes(searchTerm)
        );
        renderFriendList(filteredFriends);
    });
});


function createFriendNewList(friend) {
    const listItem = document.createElement('li');
    listItem.classList.add('tyn-aside-item', 'js-toggle-main');
    listItem.dataset.friendId = friend.uuid;
    listItem.innerHTML = `
        <div class="tyn-media-group">
            <div class="tyn-media tyn-size-lg">
                <img src="${friend.avatarUrl}" alt="">
            </div>
            <div class="tyn-media-col">
                <div class="tyn-media-row">
                    <h6 class="name">${friend.nickName}</h6>
                </div>
                <div class="tyn-media-row">
                    <p class="content">@${friend.username}</p>
                </div>
            </div>
            <div class="tyn-media-option tyn-aside-item-option">
                <ul class="tyn-media-option-list">
                    <li class="dropdown">
                        <button class="btn btn-icon btn-white btn-pill dropdown-toggle" data-bs-toggle="dropdown" data-bs-offset="0,0">
                            <!-- three-dots -->
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-three-dots" viewBox="0 0 16 16">
                                <path d="M3 9.5a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3z" />
                            </svg>
                        </button>
                        <div class="dropdown-menu dropdown-menu-end">
                            <ul class="tyn-list-links">
                                <li>
                                    <a href="index.html">
                                        <!-- chat -->
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chat" viewBox="0 0 16 16">
                                            <path d="M2.678 11.894a1 1 0 0 1 .287.801 10.97 10.97 0 0 1-.398 2c1.395-.323 2.247-.697 2.634-.893a1 1 0 0 1 .71-.074A8.06 8.06 0 0 0 8 14c3.996 0 7-2.807 7-6 0-3.192-3.004-6-7-6S1 4.808 1 8c0 1.468.617 2.83 1.678 3.894zm-.493 3.905a21.682 21.682 0 0 1-.713.129c-.2.032-.352-.176-.273-.362a9.68 9.68 0 0 0 .244-.637l.003-.01c.248-.72.45-1.548.524-2.319C.743 11.37 0 9.76 0 8c0-3.866 3.582-7 8-7s8 3.134 8 7-3.582 7-8 7a9.06 9.06 0 0 1-2.347-.306c-.52.263-1.639.742-3.468 1.105z" />
                                        </svg>
                                        <span>Send Message</span>
                                    </a>
                                </li>
                                <li class="dropdown-divider"></li>
                                <li>
                                    <a href="#">
                                        <!-- person-x -->
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-person-x" viewBox="0 0 16 16">
                                            <path d="M11 5a3 3 0 1 1-6 0 3 3 0 0 1 6 0ZM8 7a2 2 0 1 0 0-4 2 2 0 0 0 0 4Zm.256 7a4.474 4.474 0 0 1-.229-1.004H3c.001-.246.154-.986.832-1.664C4.484 10.68 5.711 10 8 10c.26 0 .507.009.74.025.226-.341.496-.65.804-.918C9.077 9.038 8.564 9 8 9c-5 0-6 3-6 4s1 1 1 1h5.256Z" />
                                            <path d="M12.5 16a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7Zm-.646-4.854.646.647.646-.647a.5.5 0 0 1 .708.708l-.647.646.647.646a.5.5 0 0 1-.708.708l-.646-.647-.646.647a.5.5 0 0 1-.708-.708l.647-.646-.647-.646a.5.5 0 0 1 .708-.708Z" />
                                        </svg>
                                        <span>Block</span>
                                    </a>
                                </li>
                                <li>
                                    <a href="#">
                                        <!-- exclamation-triangle -->
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-exclamation-triangle" viewBox="0 0 16 16">
                                            <path d="M7.938 2.016A.13.13 0 0 1 8.002 2a.13.13 0 0 1 .063.016.146.146 0 0 1 .054.057l6.857 11.667c.036.06.035.124.002.183a.163.163 0 0 1-.054.06.116.116 0 0 1-.066.017H1.146a.115.115 0 0 1-.066-.017.163.163 0 0 1-.054-.06.176.176 0 0 1 .002-.183L7.884 2.073a.147.147 0 0 1 .054-.057zm1.044-.45a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566z" />
                                            <path d="M7.002 12a1 1 0 1 1 2 0 1 1 0 0 1-2 0zM7.1 5.995a.905.905 0 1 1 1.8 0l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995z" />
                                        </svg>
                                        <span>Report</span>
                                    </a>
                                </li>
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
    `;
    return listItem;
}


document.addEventListener('DOMContentLoaded', async function () {
    loadTabBlock();

    const btBlockButton = document.getElementById('btnBlocked');

    btBlockButton.addEventListener('click', function () {
        loadTabBlock();
    });
});


async function loadTabBlock() {
    const token = await getToken();
    const response = await fetch(`${BASE_URL}api/User/getuser`, {
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    });
    const data = await response.json();
    const asideListBl = document.querySelector('#con-block');
    const searchInput = document.querySelector('#search-main');

    // Hàm để tạo danh sách người dùng bị chặn
    function renderBlockedList(users) {
        asideListBl.innerHTML = ''; // Xóa nội dung cũ
        users.forEach(user => {
            const listItem = createBlockedListItem(user);
            listItem.addEventListener('click', async function () {
                await handleListItemClick(user, listItem);
            });
            asideListBl.appendChild(listItem);
        });
    }


    renderBlockedList(data.blockedUsers);

    // Sự kiện tìm kiếm trực tiếp
    searchInput.addEventListener('input', function () {
        const searchTerm = searchInput.value.toLowerCase();
        const filteredUsers = data.blockedUsers.filter(user =>
            user.nickName.toLowerCase().includes(searchTerm) ||
            user.username.toLowerCase().includes(searchTerm)
        );
        renderBlockedList(filteredUsers);
    });
}

function createBlockedListItem(user) {
    const listItem = document.createElement('li');
    listItem.classList.add('tyn-aside-item', 'js-toggle-main');
    listItem.dataset.friendId = user.uuid;
    listItem.innerHTML = `
        <div class="tyn-media-group">
            <div class="tyn-media tyn-size-lg">
                <img src="${user.avatarUrl}" alt="">
            </div>
            <div class="tyn-media-col">
                <div class="tyn-media-row">
                    <h6 class="name">${user.nickName}</h6>
                </div>
                <div class="tyn-media-row">
                    <p class="content">@${user.username}</p>
                </div>
            </div>
            <div class="tyn-media-option tyn-aside-item-option">
                <ul class="tyn-media-option-list">
                    <li class="dropdown">
                        <button class="btn btn-icon btn-white btn-pill dropdown-toggle" data-bs-toggle="dropdown" data-bs-offset="0,0">
                            <!-- three-dots -->
                            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-three-dots" viewBox="0 0 16 16">
                                <path d="M3 9.5a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3z" />
                            </svg>
                        </button>
                        <div class="dropdown-menu dropdown-menu-end">
                            <ul class="tyn-list-links">
                                <li>
                                    <a href="#" class="unblock-user" data-uuid="${user.uuid}">
                                        <!-- person-x -->
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-person-x" viewBox="0 0 16 16">
                                            <path d="M11 5a3 3 0 1 1-6 0 3 3 0 0 1 6 0ZM8 7a2 2 0 1 0 0-4 2 2 0 0 0 0 4Zm.256 7a4.474 4.474 0 0 1-.229-1.004H3c.001-.246.154-.986.832-1.664C4.484 10.68 5.711 10 8 10c.26 0 .507.009.74.025.226-.341.496-.65.804-.918C9.077 9.038 8.564 9 8 9c-5 0-6 3-6 4s1 1 1 1h5.256Z" />
                                            <path d="M12.5 16a3.5 3.5 0 1 0 0-7 3.5 3.5 0 0 0 0 7Zm-.646-4.854.646.647.646-.647a.5.5 0 0 1 .708.708l-.647.646.647.646a.5.5 0 0 1-.708.708l-.646-.647-.646.647a.5.5 0 0 1-.708-.708l.647-.646-.647-.646a.5.5 0 0 1 .708-.708Z" />
                                        </svg>
                                        <span>UnBlock</span>
                                    </a>
                                </li>
                                <li class="dropdown-divider"></li>
                                <li>
                                    <a href="#">
                                        <!-- exclamation-triangle -->
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-exclamation-triangle" viewBox="0 0 16 16">
                                            <path d="M7.938 2.016A.13.13 0 0 1 8.002 2a.13.13 0 0 1 .063.016.146.146 0 0 1 .054.057l6.857 11.667c.036.06.035.124.002.183a.163.163 0 0 1-.054.06.116.116 0 0 1-.066.017H1.146a.115.115 0 0 1-.066-.017.163.163 0 0 1-.054-.06.176.176 0 0 1 .002-.183L7.884 2.073a.147.147 0 0 1 .054-.057zm1.044-.45a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566z" />
                                            <path d="M7.002 12a1 1 0 1 1 2 0 1 1 0 0 1-2 0zM7.1 5.995a.905.905 0 1 1 1.8 0l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995z" />
                                        </svg>
                                        <span>Report</span>
                                    </a>
                                </li>
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
    `;

    listItem.querySelector('.unblock-user').addEventListener('click', async function (event) {
        event.preventDefault();
        const uuid = this.dataset.uuid;
        const token = await getToken();

        try {
            const response = await fetch(`https://localhost:7119/api/Block/UnBlock/${uuid}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {               
                listItem.remove();
            } else {
                const errorMessage = await response.text();
                throw new Error(errorMessage);
            }
        } catch (error) {
            console.error('Error unblocking user:', error.message);
        }
    });

    return listItem;
}




document.addEventListener('DOMContentLoaded', function () {
    // Select the user search input
    const searchInput = document.querySelector('#search-username');

    // Select the ul element
    const userList = document.querySelector('#all-user-search');

    // Define variables for all users and filtered users
    let allUsers = [];
    let filteredUsers = [];

    const btnAddContact = document.getElementById('btnAddContact');

    btnAddContact.addEventListener('click', function () {
        fetchAllUsers();
    });

    async function fetchAllUsers() {
        const token = await getToken(); 

            fetch(`${BASE_URL}api/User/getAllUser`, {
                method: 'GET', // Phương thức HTTP
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            })
            .then(response => response.json())
            .then(data => {
                allUsers = data;
                renderUsers(allUsers);
            })
            .catch(error => console.error('Error fetching data:', error));
    }

    function renderUsers(users) {
        userList.innerHTML = '';
        users.forEach(user => {
            const listItem = document.createElement('li');
            const buttonContent = user.friendStatus === "Pending" ? `
                <svg width="16px" height="16px" viewBox="0 -0.5 17 17" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" class="si-glyph si-glyph-checked" data-svg="1">                   
                    <defs></defs>
                    <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
                        <path d="M3.432,6.189 C3.824,5.798 4.455,5.798 4.847,6.189 L6.968,8.31 L13.147,2.131 C13.531,1.747 14.157,1.753 14.548,2.144 L16.67,4.266 C17.06,4.657 17.066,5.284 16.684,5.666 L7.662,14.687 C7.278,15.07 6.651,15.064 6.261,14.673 L1.311,9.723 C0.92,9.333 0.92,8.7 1.311,8.31 L3.432,6.189 Z" fill="#434343" class="si-glyph-fill"></path>
                    </g>
                </svg>
            ` : `
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-person-plus-fill" viewBox="0 0 16 16" data-svg="2">
                    <path d="M1 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H1zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z" />
                    <path fill-rule="evenodd" d="M13.5 5a.5.5 0 0 1 .5.5V7h1.5a.5.5 0 0 1 0 1H14v1.5a.5.5 0 0 1-1 0V8h-1.5a.5.5 0 0 1 0-1H13V5.5a.5.5 0 0 1 .5-.5z" />
                </svg>
            `;

            listItem.innerHTML = `
                <div class="tyn-media-group">
                    <div class="tyn-media">
                        <img src="${user.avatarUrl}" alt="">
                    </div>
                    <div class="tyn-media-col">
                        <div class="tyn-media-row">
                            <h6 class="name">${user.nickName}</h6>
                        </div>
                        <div class="tyn-media-row">
                            <p class="content">@${user.userName}</p>
                        </div>
                    </div>
                    <ul class="tyn-media-option-list me-n1">
                        <li class="dropdown">
                            <button class="btn btn-icon btn-white btn-pill SendAdd" data-uuid="${user.uuid}" data-status="${user.friendStatus}">
                                ${buttonContent}
                            </button>
                        </li>
                    </ul>
                </div><!-- .tyn-media-group -->
            `;
            userList.appendChild(listItem);

            // Add click event listener for each SendAdd button
            const sendAddButton = listItem.querySelector('.SendAdd');
            sendAddButton.addEventListener('click', function () {
                const uuid = sendAddButton.getAttribute('data-uuid');
                const status = sendAddButton.getAttribute('data-status');
                if (status === "Pending" || status === "Rejected") {
                    updateFriendStatus(uuid, sendAddButton);
                } else {
                    addFriend(uuid, sendAddButton);
                }
            });
        });
    }

    // Function to add a friend using API
    async function addFriend(uuid, button) {
        const token = await getToken();
        try {
            const response = await fetch(`${BASE_URL}api/Friend/AddFriend/${uuid}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                // body: JSON.stringify({ uuid: uuid }) // You can send data if needed
            });

            if (!response.ok) {
                const errorMessage = await response.text();
                throw new Error(errorMessage);
            }

            // Handle success if needed
            console.log('Friend request sent successfully.');
            toggleButtonSVG(button);
            const userId = uuid;
            const message = "You have received a friend request!";
            connection.invoke("SendFriendRequestNotification", userId, message)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        } catch (error) {
            console.error('Error sending friend request:', error.message);
        }
    }

    // Function to update friend status using API
    async function updateFriendStatus(uuidf, button) {
        var user = await getUser();
        const token = await getToken();
        try {
            const response = await fetch(`${BASE_URL}api/Friend/UpdateStatusFriendShip/${user.uuid}/${uuidf}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                
            });

            if (!response.ok) {
                const errorMessage = await response.text();
                throw new Error(errorMessage);
            }

            // Handle success if needed
            console.log('Friend status updated successfully.');
            toggleButtonSVG(button);
        } catch (error) {
            console.error('Error updating friend status:', error.message);
        }
    }

    // Function to toggle SVG content
    function toggleButtonSVG(button) {
        const svg1 = `
            <svg width="16px" height="16px" viewBox="0 -0.5 17 17" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" class="si-glyph si-glyph-checked" data-svg="1">                   
                <defs></defs>
                <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
                    <path d="M3.432,6.189 C3.824,5.798 4.455,5.798 4.847,6.189 L6.968,8.31 L13.147,2.131 C13.531,1.747 14.157,1.753 14.548,2.144 L16.67,4.266 C17.06,4.657 17.066,5.284 16.684,5.666 L7.662,14.687 C7.278,15.07 6.651,15.064 6.261,14.673 L1.311,9.723 C0.92,9.333 0.92,8.7 1.311,8.31 L3.432,6.189 Z" fill="#434343" class="si-glyph-fill"></path>
                </g>
            </svg>
        `;
        const svg2 = `
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-person-plus-fill" viewBox="0 0 16 16" data-svg="2">
                <path d="M1 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H1zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z" />
                <path fill-rule="evenodd" d="M13.5 5a.5.5 0 0 1 .5.5V7h1.5a.5.5 0 0 1 0 1H14v1.5a.5.5 0 0 1-1 0V8h-1.5a.5.5 0 0 1 0-1H13V5.5a.5.5 0 0 1 .5-.5z" />
            </svg>
        `;

        // Check the current SVG using data attribute
        const currentSVG = button.querySelector('svg').getAttribute('data-svg');
        if (currentSVG === '1') {
            button.innerHTML = svg2;
        } else {
            button.innerHTML = svg1;
        }
    }

    // Fetch all users when the page loads
    fetchAllUsers();

    // Add input event listener to searchInput
    searchInput.addEventListener('input', function () {
        const searchTerm = searchInput.value.toLowerCase();
        filteredUsers = allUsers.filter(user => user.nickName.toLowerCase().includes(searchTerm));
        renderUsers(filteredUsers); // Render filtered users
    });

});




document.addEventListener('DOMContentLoaded', function () {
    const galleryButton = document.querySelector('#gallery-click');

    galleryButton.addEventListener('click', function () {
        fetch(`${BASE_URL}api/Story/${selectedFriendUUID}`)
            .then(response => response.json())
            .then(stories => {
                const galleryDiv = document.querySelector('#profile-gallery .row');
                galleryDiv.innerHTML = '';

                if (stories.length === 0) {
                    galleryDiv.innerHTML = '<p>There are no stories</p>';
                } else {
                    stories.forEach(story => {
                        const storyItem = document.createElement('div');
                        storyItem.classList.add('col-xxl-2', 'col-xl-3', 'col-lg-4', 'col-6');
                        storyItem.innerHTML = `
                            <img src="${story.path}" class="tyn-image" alt="">
                        `;
                        galleryDiv.appendChild(storyItem);
                    });
                }
            })
            .catch(error => console.error('Error fetching stories:', error));
    });
});


async function loadGallery(userUUID) {
    try {
        const response = await fetch(`${BASE_URL}api/Story/${userUUID}`);
        const stories = await response.json();
        const galleryDiv = document.querySelector('#profile-gallery .row');
        galleryDiv.innerHTML = ''; // Clear previous content

        if (stories.length === 0) {
            galleryDiv.innerHTML = '<p>There are no stories</p>';
        } else {
            stories.forEach(story => {
                const storyItem = document.createElement('div');
                storyItem.classList.add('col-xxl-2', 'col-xl-3', 'col-lg-4', 'col-6');
                storyItem.innerHTML = `
                    <img src="${story.path}" class="tyn-image" alt="">
                `;
                galleryDiv.appendChild(storyItem);
            });
        }
    } catch (error) {
        console.error('Error fetching stories:', error);
    }
}



document.addEventListener('DOMContentLoaded', function () {
    const storiesButton = document.querySelector('#stories-click');

    storiesButton.addEventListener('click', function () {
        fetch(`${BASE_URL}api/Story/${selectedFriendUUID}`)
            .then(response => response.json())
            .then(stories => {
                const lastThreeStories = stories.slice(-3).reverse();
                const imageLargeDiv = document.querySelector('#image-large');
                imageLargeDiv.innerHTML = ''; // Clear previous content
                lastThreeStories.forEach(story => {
                    const slideItem = document.createElement('div');
                    slideItem.classList.add('swiper-slide');
                    slideItem.innerHTML = `
                <div class="tyn-stories-item">
                    <img src="${story.path}" class="tyn-image" alt="">
                    <div class="tyn-stories-content">
                        <h5 class="tyn-stories-title text-white">Tuan DV</h5>
                        <p class="text-white">#boating, #ohio</p>
                    </div>
                </div>
            `;
                    imageLargeDiv.appendChild(slideItem);
                });

                // Chèn vào image-small
                const imageSmallDiv = document.querySelector('#image-small');
                imageSmallDiv.innerHTML = ''; // Clear previous content
                lastThreeStories.forEach(story => {
                    const slideItem = document.createElement('div');
                    slideItem.classList.add('swiper-slide');
                    slideItem.innerHTML = `
                <img src="${story.path}" class="tyn-image" alt="">
            `;
                    imageSmallDiv.appendChild(slideItem);
                });
            })
            .catch(error => console.error('Error fetching stories:', error));
    });
});



async function loadStories(userUUID) {
    try {
        
        const response = await fetch(`${BASE_URL}api/Story/${userUUID}`);
        const stories = await response.json();

        const lastThreeStories = stories.slice(-3).reverse();
        // Chèn vào image-large
        const imageLargeDiv = document.querySelector('#image-large');
        imageLargeDiv.innerHTML = ''; // Clear previous content
        lastThreeStories.forEach(story => {
            const slideItem = document.createElement('div');
            slideItem.classList.add('swiper-slide');
            slideItem.innerHTML = `
                <div class="tyn-stories-item">
                    <img src="${story.path}" class="tyn-image" alt="">
                    <div class="tyn-stories-content">
                        <h5 class="tyn-stories-title text-white">Tuan DV</h5>
                        <p class="text-white">#boating, #ohio</p>
                    </div>
                </div>
            `;
            imageLargeDiv.appendChild(slideItem);
        });

        
        const imageSmallDiv = document.querySelector('#image-small');
        imageSmallDiv.innerHTML = ''; // Clear previous content
        lastThreeStories.forEach(story => {
            const slideItem = document.createElement('div');
            slideItem.classList.add('swiper-slide');
            slideItem.innerHTML = `
                <img src="${story.path}" class="tyn-image" alt="">
            `;
            imageSmallDiv.appendChild(slideItem);
        });
    } catch (error) {
        console.error('Error fetching stories:', error);
    }
}



document.addEventListener('DOMContentLoaded',async function () {
    const mutualButton = document.querySelector('#mutual-click');
    var user = await getUser();
    let uui1 = user.uuid;
    mutualButton.addEventListener('click', function () {
        fetch(`${BASE_URL}api/User/GetMutualFriends/${uui1}/${selectedFriendUUID}`)
            .then(response => response.json())
            .then(mutualFriends => {
                const mutualContactDiv = document.querySelector('#mutual-contact');
                mutualContactDiv.innerHTML = ''; // Clear previous content

                if (mutualFriends.length === 0) {
                    mutualContactDiv.innerHTML = '<p>There are no common friends</p>';
                } else {
                    mutualFriends.forEach(friend => {
                        const friendItem = document.createElement('div');
                        friendItem.classList.add('col-xxl-2', 'col-xl-3', 'col-lg-4', 'col-sm-6');
                        friendItem.innerHTML = `
                            <div class="border border-light rounded-3 py-4 px-3 h-100 d-flex flex-column align-items-center justify-content-center text-center">
                                <div class="tyn-media tyn-size-2xl tyn-circle mb-3">
                                    <img src="${friend.avatarUrl}" alt="">
                                </div>
                                <span class="tyn-subtext mb-1">@${friend.userName}</span>
                                <h6>${friend.nickName}</h6>
                            </div>
                        `;
                        mutualContactDiv.appendChild(friendItem);
                    });
                }
            })
            .catch(error => console.error('Error fetching mutual friends:', error));
    });
});

async function loadMutual(userUUID) {
    try {
        var user = await getUser();
        let uui1 = user.uuid;
        const response = await fetch(`${BASE_URL}api/User/GetMutualFriends/${uui1}/${userUUID}`);
        const mutualFriends = await response.json();

        const mutualContactDiv = document.querySelector('#mutual-contact');
        mutualContactDiv.innerHTML = ''; // Clear previous content

        if (mutualFriends.length === 0) {
            mutualContactDiv.innerHTML = '<p>There are no common friends</p>';
        } else {
            mutualFriends.forEach(friend => {
                const friendItem = document.createElement('div');
                friendItem.classList.add('col-xxl-2', 'col-xl-3', 'col-lg-4', 'col-sm-6');
                friendItem.innerHTML = `
                    <div class="border border-light rounded-3 py-4 px-3 h-100 d-flex flex-column align-items-center justify-content-center text-center">
                        <div class="tyn-media tyn-size-2xl tyn-circle mb-3">
                            <img src="${friend.avatarUrl}" alt="">
                        </div>
                        <span class="tyn-subtext mb-1">@${friend.userName}</span>
                        <h6>${friend.nickName}</h6>
                    </div>
                `;
                mutualContactDiv.appendChild(friendItem);
            });
        }
    } catch (error) {
        console.error('Error fetching mutual friends:', error);
    }
}


