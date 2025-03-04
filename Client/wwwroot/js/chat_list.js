
document.addEventListener("DOMContentLoaded", async function () {

    

    const token = await getToken();

    if (!token) {
        console.error("Token is required to fetch chat data.");
        return;
    }

    const url = `${BASE_URL}api/Chat/chats_user`;
    console.log(url);

    try {
        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
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
        const chatList = document.getElementById("chat-list");
        const individualChatList = document.getElementById("individual-chat-list");
        const groupChatList = document.getElementById("group-chat-list");

        chatList.innerHTML = ""; 
        individualChatList.innerHTML = "";
        groupChatList.innerHTML = "";

        let currentIndex = 0;
        const itemsPerPage = 10;

        function loadMoreItems() {
            const nextIndex = currentIndex + itemsPerPage;
            const itemsToLoad = data.slice(currentIndex, nextIndex);

            itemsToLoad.forEach((chat) => {
                const chatItem = document.createElement("li");
                chatItem.classList.add("tyn-aside-item", "js-toggle-main", "custom-cursor-on-hover");
                chatItem.id = chat.uuid;
                console.log("D: " + chat.uuid);

                const lastMessageContent = chat.lastMessage
                    ? chat.lastMessage.content
                    : "";
                const lastMessageTime = chat.lastMessage
                    ? new Date(chat.lastMessage.sentTime)
                    : new Date();
                const lastMessageIsRead = chat.lastMessage
                    ? chat.lastMessage.isRead
                    : true;

                const messageClass = lastMessageIsRead ? "read" : "unread";

                // Kiểm tra IsGroup để lấy avatar và name phù hợp
                const avatarUrl = chat.isGroup ? chat.avatarUrl : chat.otherUser.avatarUrl;
                const chatName = chat.isGroup ? chat.chatName : chat.otherUser.nickName;

                chatItem.innerHTML = `
                    <div class="tyn-media-group">
                        <div class="tyn-media tyn-size-lg">
                            <img src="${avatarUrl}" alt="">
                        </div>
                        <div class="tyn-media-col">
                            <div class="tyn-media-row">
                                <h6 class="name">${chatName}</h6>
                                <div class="indicator varified">
                                    <!-- check-circle-fill -->
                                </div>
                            </div>
                            <div class="tyn-media-row has-dot-sap">
                                <p class="content ${messageClass}">${lastMessageContent}</p>
                                <span class="meta">${timeSince(lastMessageTime)}</span>
                            </div>
                        </div>
                        <div class="tyn-media-option tyn-aside-item-option">
                            <ul class="tyn-media-option-list">
                                <li class="dropdown">
                                    <button class="btn btn-icon btn-white btn-pill dropdown-toggle" data-bs-toggle="dropdown" data-bs-offset="0,0" data-bs-auto-close="outside">
                                        <!-- three-dots -->
                                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-three-dots" viewBox="0 0 16 16">
                                            <path d="M3 9.5a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3z"/>
                                        </svg>
                                    </button>
                                <div class="dropdown-menu dropdown-menu-end">
                                    <ul class="tyn-list-links">
                                        <li>
                                            <a href="#">
                                                <!-- check -->
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-check" viewBox="0 0 16 16">
                                                    <path d="M10.97 4.97a.75.75 0 0 1 1.07 1.05l-3.99 4.99a.75.75 0 0 1-1.08.02L4.324 8.384a.75.75 0 1 1 1.06-1.06l2.094 2.093 3.473-4.425a.267.267 0 0 1 .02-.022z" />
                                                </svg>
                                                <span>Mark as Read</span>
                                            </a>
                                        </li>
                                        <li>
                                            <a href="#">
                                                <!-- bell -->
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-bell" viewBox="0 0 16 16">
                                                    <path d="M8 16a2 2 0 0 0 2-2H6a2 2 0 0 0 2 2zM8 1.918l-.797.161A4.002 4.002 0 0 0 4 6c0 .628-.134 2.197-.459 3.742-.16.767-.376 1.566-.663 2.258h10.244c-.287-.692-.502-1.49-.663-2.258C12.134 8.197 12 6.628 12 6a4.002 4.002 0 0 0-3.203-3.92L8 1.917zM14.22 12c.223.447.481.801.78 1H1c.299-.199.557-.553.78-1C2.68 10.2 3 6.88 3 6c0-2.42 1.72-4.44 4.005-4.901a1 1 0 1 1 1.99 0A5.002 5.002 0 0 1 13 6c0 .88.32 4.2 1.22 6z" />
                                                </svg>
                                                <span>Mute Notifications</span>
                                            </a>
                                        </li>
                                        <li>
                                            <a href="contacts.html">
                                                <!-- person -->
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-person" viewBox="0 0 16 16">
                                                    <path d="M8 8a3 3 0 1 0 0-6 3 3 0 0 0 0 6Zm2-3a2 2 0 1 1-4 0 2 2 0 0 1 4 0Zm4 8c0 1-1 1-1 1H3s-1 0-1-1 1-4 6-4 6 3 6 4Zm-1-.004c-.001-.246-.154-.986-.832-1.664C11.516 10.68 10.289 10 8 10c-2.29 0-3.516.68-4.168 1.332-.678.678-.83 1.418-.832 1.664h10Z" />
                                                </svg>
                                                <span>View Profile</span>
                                            </a>
                                        </li>
                                        <li>
                                            <a href="#" class="delete-chat" data-chat-id="${chat.uuid}">
                                                <!-- trash -->
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash" viewBox="0 0 16 16">
                                                    <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6Z" />
                                                    <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1ZM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118ZM2.5 3h11V2h-11v1Z" />
                                                </svg>
                                                <span>Delete</span>
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
                
                chatList.appendChild(chatItem);
                if (chat.isGroup) {
                    groupChatList.appendChild(chatItem.cloneNode(true));
                } else {
                    individualChatList.appendChild(chatItem.cloneNode(true));
                }
            });

            currentIndex = nextIndex;

            // Observe the last item
            const lastItem = chatList.querySelector("li:last-child");
            if (lastItem) {
                observer.observe(lastItem);
            }
        }

        const observer = new IntersectionObserver((entries) => {
            if (entries[0].isIntersecting) {
                observer.unobserve(entries[0].target);
                loadMoreItems();
            }
        }, {
            root: null,
            rootMargin: "0px",
            threshold: 1.0
        });

        // Initial load
        loadMoreItems();

        // Event delegation for click event on chat items
        document.addEventListener("click", function (event) {
            const chatItem = event.target.closest(".tyn-aside-item.js-toggle-main");
            if (chatItem) {
                console.log("Chat item clicked:", chatItem); // Debug log
                // Remove 'active' class from all items
                document
                    .querySelectorAll(".tyn-aside-item.js-toggle-main")
                    .forEach((item) => {
                        item.classList.remove("active");
                    });
                // Add 'active' class to the clicked item
                chatItem.classList.add("active");
                console.log("Active class added to:", chatItem); // Debug log
            }
        });

        // Event delegation for delete chat
        document.addEventListener("click", async function (event) {
            const deleteButton = event.target.closest(".delete-chat");
            if (deleteButton) {
                const chatId = deleteButton.getAttribute("data-chat-id");
                if (!chatId) {
                    console.error("Chat ID is undefined");
                    return;
                }
                const confirmed = confirm("Are you sure you want to delete this chat?");
                if (confirmed) {
                    try {
                        const deleteUrl = `${BASE_URL}api/Chat/delete_chat/${chatId}`;
                        const deleteResponse = await fetch(deleteUrl, {
                            method: "DELETE",
                            headers: {
                                "Content-Type": "application/json",
                                Authorization: `Bearer ${token}`,
                            },
                        });

                        if (!deleteResponse.ok) {
                            console.error("Network response was not ok", deleteResponse.status, deleteResponse.statusText);
                            throw new Error("Network response was not ok " + deleteResponse.statusText);
                        }

                        // Remove the chat item from the DOM
                        const chatItem = document.getElementById(chatId);
                        if (chatItem) {
                            chatItem.remove();
                        }
                        alert("Chat deleted successfully!");
                        // Reload the page
                        //location.reload();
                        
                    } catch (error) {
                        console.error("Error deleting chat:", error);
                    }
                }
            }
        });

        // Thêm sự kiện tìm kiếm
        const searchInput = document.getElementById("search");
        searchInput.addEventListener("input", function () {
            const searchTerm = searchInput.value.toLowerCase();
            const chatItems = document.querySelectorAll(
                ".tyn-aside-item.js-toggle-main"
            );

            chatItems.forEach((item) => {
                const chatName = item.querySelector(".name").textContent.toLowerCase();
                if (chatName.includes(searchTerm)) {
                    item.style.display = "";
                } else {
                    item.style.display = "none";
                }
            });
        });

        connection.on("UpdateChatList", () => {
            // Reload chat list when notified by server
            loadMoreItems();
        });

        // Lấy tất cả các thẻ <li> và so sánh với chatUuid
        const chatContainer1 = document.getElementById('chat-chatUuid');
        const chatContainer2 = document.getElementById('chat-uuidU');
        const chatContainer3 = document.getElementById('chat-uuidF');

        const chatUuid = chatContainer1.getAttribute('data-chatUuid');
        const chatUuidU = chatContainer2.getAttribute('data-uuidU');
        const chatUuidF = chatContainer3.getAttribute('data-uuidF');

        const chatItems = document.querySelectorAll(".tyn-aside-item.js-toggle-main");
        console.log("Chatuuid: " + chatUuid);
        console.log("uuidU: " + chatUuidU);
        console.log("uuidF: " + chatUuidF);
        
        chatItems.forEach((item) => {
            
            if (item.id === chatUuid) {
                item.classList.add("active");
                console.log("Active");               
            }
        });

        if (chatUuid === "Default") {
            console.log("In");
            getUUID(chatUuidU, chatUuidF);
        }
    } catch (error) {
        console.error("Error fetching chat data:", error);
    }

});

async function getUUID(uuidU, uuidF) {
    const token = await getToken();

    if (!token) {
        console.error("Token is required to fetch chat data.");
        return;
    }

    const url = `https://localhost:7119/api/Chat/CreateChatMessage/${uuidU}/${uuidF}`;

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
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
        console.log("UUID data:", data);
        setTimeout(() => {
            window.location.reload();
        }, 100);
    } catch (error) {
        console.error("Error fetching UUID data:", error);
    }
}

function timeSince(date) {
    const seconds = Math.floor((new Date() - date) / 1000);
    let interval = Math.floor(seconds / 31536000);

    if (interval > 1) {
        return interval + " years";
    }
    interval = Math.floor(seconds / 2592000);
    if (interval > 1) {
        return interval + " months";
    }
    interval = Math.floor(seconds / 86400);
    if (interval > 1) {
        return interval + " days";
    }
    interval = Math.floor(seconds / 3600);
    if (interval > 1) {
        return interval + " hours";
    }
    interval = Math.floor(seconds / 60);
    if (interval > 1) {
        return interval + " minutes";
    }
    return Math.floor(seconds) + " seconds";
}