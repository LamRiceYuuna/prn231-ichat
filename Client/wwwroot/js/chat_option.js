document.addEventListener('DOMContentLoaded', async function () {
    const individualChat = document.getElementById('individualChat');
    const groupChat = document.getElementById('groupChat');
    const contactList = document.getElementById('contact-list');
    const createChatBtn = document.getElementById('createChatBtn');
    const searchContact = document.getElementById('search-contact');

    let selectedFriend = null; // Biến để lưu friend được chọn
    let friends = []; // Biến để lưu danh sách bạn bè
    let currentIndex = 0;
    const itemsPerPage = 6;

    async function fetchFriends(chatType) {
        const token = await getToken();
        if (!token) {
            console.error("Token is required to fetch friends data.");
            return;
        }

        const url = `${BASE_URL}api/FriendShip/friends_for_chat?chatType=${chatType}`;

        try {
            const response = await fetch(url, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
            });

            if (!response.ok) {
                console.error("Network response was not ok", response.status, response.statusText);
                throw new Error("Network response was not ok " + response.statusText);
            }

            friends = await response.json();
            contactList.innerHTML = ""; // Clear existing list
            currentIndex = 0;
            loadMoreFriends();
        } catch (error) {
            console.error("Error fetching friends data:", error);
        }
    }

    function loadMoreFriends() {
        const nextIndex = currentIndex + itemsPerPage;
        const itemsToLoad = friends.slice(currentIndex, nextIndex);

        itemsToLoad.forEach(friend => {
            const listItem = document.createElement('li');
            listItem.classList.add('tyn-media-group');
            listItem.innerHTML = `
                <div class="tyn-media">
                    <img src="${friend.avatarUrl}" alt="">
                </div>
                <div class="tyn-media-col">
                    <div class="tyn-media-row">
                        <h6 class="name">${friend.nickName}</h6>
                    </div>
                </div>
            `;
            if (groupChat.checked) {
                const checkboxDiv = document.createElement('div');
                checkboxDiv.className = 'form-check';
                checkboxDiv.innerHTML = `
                    <input class="form-check-input" type="checkbox" value="${friend.nickName}">
                `;
                listItem.appendChild(checkboxDiv);
            }
            listItem.addEventListener('click', () => selectFriend(friend, listItem));
            contactList.appendChild(listItem);
        });

        currentIndex = nextIndex;

        if (currentIndex < friends.length) {
            const lastItem = contactList.lastElementChild;
            observer.observe(lastItem);
        }
    }

    const observer = new IntersectionObserver((entries) => {
        if (entries[0].isIntersecting) {
            observer.unobserve(entries[0].target);
            loadMoreFriends();
        }
    }, {
        root: null,
        rootMargin: "0px",
        threshold: 1.0
    });

    function selectFriend(friend, listItem) {
        if (individualChat.checked) {
            // Xóa class 'selected' khỏi tất cả các friend
            const allFriends = contactList.querySelectorAll('li');
            allFriends.forEach(item => item.classList.remove('selected'));

            // Đánh dấu friend được chọn
            listItem.classList.add('selected');
            selectedFriend = friend;
        }
    }

    function updateCheckboxes() {
        const listItems = contactList.querySelectorAll('li');
        listItems.forEach(li => {
            if (groupChat.checked) {
                if (!li.querySelector('.form-check')) {
                    const checkboxDiv = document.createElement('div');
                    checkboxDiv.className = 'form-check';
                    checkboxDiv.innerHTML = `
                        <input class="form-check-input" type="checkbox" value="${li.querySelector('.name').textContent}">
                    `;
                    li.appendChild(checkboxDiv);
                }
            } else {
                const checkboxDiv = li.querySelector('.form-check');
                if (checkboxDiv) {
                    checkboxDiv.remove();
                }
            }
        });
    }

    function filterFriends() {
        const searchTerm = searchContact.value.toLowerCase();
        const listItems = contactList.querySelectorAll('li');

        listItems.forEach(li => {
            const name = li.querySelector('.name').textContent.toLowerCase();
            if (name.includes(searchTerm)) {
                li.style.display = '';
            } else {
                li.style.display = 'none';
            }
        });
    }

    async function createNewChat() {
        const selectedFriends = [];
        const chatType = document.querySelector('input[name="chatType"]:checked').value;

        if (chatType === 'individual') {
            if (selectedFriend) {
                selectedFriends.push(selectedFriend.userId); // Sử dụng userId thay vì nickName
            } else {
                alert("Please select a friend to create an individual chat.");
                return;
            }
        } else if (chatType === 'group') {
            const selectedCheckboxes = contactList.querySelectorAll('li .form-check-input:checked');
            selectedCheckboxes.forEach(checkbox => {
                const friend = friends.find(f => f.nickName === checkbox.value);
                if (friend) {
                    selectedFriends.push(friend.userId); // Sử dụng userId thay vì nickName
                }
            });

            if (selectedFriends.length < 2) {
                alert("Please select at least 2 friends to create a group chat.");
                return;
            }
        }

        const token = await getToken();
        if (!token) {
            console.error("Token is required to create a new chat.");
            return;
        }

        const url = `${BASE_URL}api/Chat/create_chat`;
        const payload = {
            chatType: chatType,
            friends: selectedFriends
        };

        try {
            const response = await fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                console.error("Network response was not ok", response.status, response.statusText);
                throw new Error("Network response was not ok " + response.statusText);
            }

            const result = await response.json();
            alert("Chat created successfully!");
            // Close the modal
            $('#newChat').modal('hide');
            // Load lại trang sau khi hiển thị thông báo
            setTimeout(() => {
                window.location.reload();
            }, 100); // Đợi 1 giây trước khi load lại trang
        } catch (error) {
            console.error("Error creating new chat:", error);
        }
    }

    individualChat.addEventListener('change', () => {
        fetchFriends('individual');
        updateCheckboxes();
    });
    groupChat.addEventListener('change', () => {
        fetchFriends('group');
        updateCheckboxes();
    });
    createChatBtn.addEventListener('click', createNewChat);
    searchContact.addEventListener('input', filterFriends);

    fetchFriends('individual'); // Fetch friends when the modal is opened with default chat type
});