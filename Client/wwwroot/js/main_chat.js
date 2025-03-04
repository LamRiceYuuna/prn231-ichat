var chatUUID = null; //gán bằng null

document.addEventListener("DOMContentLoaded", () => {
    
    let simpleBody = new SimpleBar(document.querySelector('#tynChatBody'));
    let scrollElement = simpleBody.getScrollElement();
    scrollElement.scrollTop = scrollElement.scrollHeight - scrollElement.clientHeight;

    scrollElement.addEventListener('scroll',async function (event) {
        if (scrollElement.scrollTop === 0) {
            await loadLazyMessage(lastClickedElement);
        }
    });

    let lastClickedElement = null;
    function handleActiveClassAdded(element) {
        if (lastClickedElement !== element) {
            chatUUID = element.id; //bỏ comment ra
            console.log("chatUUID" + chatUUID);
            lastClickedElement = element;
            const searchInThisChat = document.querySelector("#searchInThisChat");
            searchInThisChat.value = "";
            currentIndexSearch = 0;
            dictionary = null;
            onStatusChat();
            showMessageChat(element);
        }
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

    function addClickEventAndObserver(item) {
        let isHandling = false;

        item.addEventListener("click", () => {
            if (!isHandling && !item.classList.contains("active")) {
                isHandling = true;
                handleActiveClassAdded(item);
                setTimeout(() => { isHandling = false; }, 0);
            }
        });

        const observer = new MutationObserver(mutations => {
            mutations.forEach(mutation => {
                if (mutation.attributeName === "class") {
                    const currentClassState = mutation.target.classList.contains("active");
                    if (currentClassState) {
                        handleActiveClassAdded(mutation.target);
                    }
                }
            });
        });

        observer.observe(item, { attributes: true });
    }

    const items = document.querySelectorAll("li.tyn-aside-item.js-toggle-main");
    items.forEach(addClickEventAndObserver);

    const containerObserver = new MutationObserver(mutations => {
        mutations.forEach(mutation => {
            mutation.addedNodes.forEach(node => {
                if (node.nodeType === Node.ELEMENT_NODE && node.matches("li.tyn-aside-item.js-toggle-main")) {
                    addClickEventAndObserver(node);
                }
            });
        });
    });

    const containers = document.querySelectorAll(".tyn-aside-list");
    containers.forEach(container => {
        containerObserver.observe(container, { childList: true, subtree: true });
    });

    if (containers.length === 0) {
        console.error("Không tìm thấy container nào với class 'tyn-aside-list'.");
    }

    function createChatAction(messageUUID) {
    return `
          <ul class="tyn-reply-tools">
            <li>
                <button class="btn btn-icon btn-sm btn-transparent btn-pill">
                    <!-- emoji-smile-fill -->
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-emoji-smile-fill" viewBox="0 0 16 16">
                        <path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zM7 6.5C7 7.328 6.552 8 6 8s-1-.672-1-1.5S5.448 5 6 5s1 .672 1 1.5zM4.285 9.567a.5.5 0 0 1 .683.183A3.498 3.498 0 0 0 8 11.5a3.498 3.498 0 0 0 3.032-1.75.5.5 0 1 1 .866.5A4.498 4.498 0 0 1 8 12.5a4.498 4.498 0 0 1-3.898-2.25.5.5 0 0 1 .183-.683zM10 8c-.552 0-1-.672-1-1.5S9.448 5 10 5s1 .672 1 1.5S10.552 8 10 8z" />
                    </svg>
                </button>
            </li><!-- li -->
            <li class="dropup-center">
                <button class="btn btn-icon btn-sm btn-transparent btn-pill" data-bs-toggle="dropdown">
                    <!-- three-dots -->
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-three-dots" viewBox="0 0 16 16">
                        <path d="M3 9.5a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3zm5 0a1.5 1.5 0 1 1 0-3 1.5 1.5 0 0 1 0 3z" />
                    </svg>
                </button>
                <div class="dropdown-menu dropdown-menu-xxs">
                    <ul class="tyn-list-links">
                        <li>
                            <a href="#" class = "edit-message" data-message-uuid = "${messageUUID}">
                                <!-- pencil-square -->
                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-pencil-square" viewBox="0 0 16 16">
                                    <path d="M15.502 1.94a.5.5 0 0 1 0 .706L14.459 3.69l-2-2L13.502.646a.5.5 0 0 1 .707 0l1.293 1.293zm-1.75 2.456-2-2L4.939 9.21a.5.5 0 0 0-.121.196l-.805 2.414a.25.25 0 0 0 .316.316l2.414-.805a.5.5 0 0 0 .196-.12l6.813-6.814z" />
                                    <path fill-rule="evenodd" d="M1 13.5A1.5 1.5 0 0 0 2.5 15h11a1.5 1.5 0 0 0 1.5-1.5v-6a.5.5 0 0 0-1 0v6a.5.5 0 0 1-.5.5h-11a.5.5 0 0 1-.5-.5v-11a.5.5 0 0 1 .5-.5H9a.5.5 0 0 0 0-1H2.5A1.5 1.5 0 0 0 1 2.5v11z" />
                                </svg>
                                <span>Edit</span>
                            </a>
                        </li>
                        <li>
                            <a href="#" class = "delete-message" data-message-uuid = "${messageUUID}">
                                <!-- trash -->
                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash" viewBox="0 0 16 16">
                                    <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5Zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6Z" />
                                    <path d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1ZM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118ZM2.5 3h11V2h-11v1Z" />
                                </svg>
                                <span>Delete</span>
                            </a>
                        </li>
                    </ul><!-- .tyn-list-links -->
                </div><!-- .dropdown-menu -->
            </li><!-- li -->
        </ul><!-- .tyn-reply-tools -->
    `
    }

    function createMessageVideo(path) {
        return `
            <div class="tyn-reply-media">
                <a href="${path}" class="glightbox tyn-video" data-gallery="media-video">
                    <img src="../images/gallery/video/background-video.jpg" class="tyn-image" alt="">
                    <div class="tyn-video-icon">
                        <!-- play-fill -->
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-play-fill" viewBox="0 0 16 16">
                            <path d="m11.596 8.697-6.363 3.692c-.54.313-1.233-.066-1.233-.697V4.308c0-.63.692-1.01 1.233-.696l6.363 3.692a.802.802 0 0 1 0 1.393z" />
                        </svg>
                    </div>
                </a>
            </div>
        `
    }
    function createMessageDocument(path, name) {
        return `
            <div class="tyn-reply-file">
                <a href="${path}" class="tyn-file">
                    <div class="tyn-media-group">
                        <div class="tyn-media tyn-size-lg text-bg-light">
                            <!-- filetype-docx -->
                            <svg xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" width="100" height="100" viewBox="0 0 50 50">
                                <path d="M 29.984375 1.9863281 A 1.0001 1.0001 0 0 0 29.839844 2 L 7 2 L 7 48 L 43 48 L 43 47 L 43 15.167969 A 1.0001 1.0001 0 0 0 43 14.841797 L 43 14.585938 L 42.806641 14.392578 A 1.0001 1.0001 0 0 0 42.623047 14.207031 A 1.0001 1.0001 0 0 0 42.617188 14.203125 L 30.791016 2.3769531 A 1.0001 1.0001 0 0 0 30.603516 2.1894531 L 30.414062 2 L 30.154297 2 A 1.0001 1.0001 0 0 0 29.984375 1.9863281 z M 9 4 L 29 4 L 29 16 L 41 16 L 41 46 L 9 46 L 9 4 z M 31 5.4140625 L 39.585938 14 L 31 14 L 31 5.4140625 z M 15 22 L 15 24 L 35 24 L 35 22 L 15 22 z M 15 28 L 15 30 L 31 30 L 31 28 L 15 28 z M 15 34 L 15 36 L 35 36 L 35 34 L 15 34 z"></path>
                            </svg>
                        </div>
                        <div class="tyn-media-col">
                            <h6 class="name">${name}</h6>
                            <div class="meta">24.65 MB</div>
                        </div>
                    </div>
                </a>
            </div>
            `
    }
    function createMessageImage(path) {
        return  `
            <a href="${path}" class="glightbox tyn-thumb" data-gallery="media-photo">
                <img src="${path}" class="tyn-image" alt="">
            </a>
         `
    }

    function createChatIncoming(messageUUID, avatarUrl, contentMessage, nickname) {
        return `
            <div class="tyn-reply-item incoming" id = ${messageUUID}>
                <div class="tyn-reply-avatar">
                    <div class="tyn-media tyn-size-md tyn-circle">
                        <img src="${avatarUrl}" alt="">
                    </div>
                </div>
                <div class="tyn-reply-group">
                <a href="#" style="font-size: 12px;">${nickname}</a>
                    <div class="tyn-reply-bubble">
                        ${contentMessage}
                    </div><!-- .tyn-reply-bubble -->
                </div><!-- .tyn-reply-group -->
            </div><!-- .tyn-reply-item -->`
    }

    function createChatOutgoing(messageUUID, contentMessage) {
        return `
            <div class="tyn-reply-item outgoing" id = ${messageUUID}>
                <div class="tyn-reply-group">
                    <div class="tyn-reply-bubble">
                        ${contentMessage}
                        ${createChatAction(messageUUID)}
                    </div><!-- .tyn-reply-bubble -->
                </div><!-- .tyn-reply-group -->
                </div><!-- .tyn-reply-item -->
            `
    }
    function formatDate(dateString) {
        const options = {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: 'numeric',
            minute: 'numeric'
        };
        return new Date(dateString).toLocaleDateString(undefined, options);
    }
    function refreshLightbox() {
        const lightbox = GLightbox({
            selector: '.glightbox'
        });
    }
    async function showMessageChat(element) {
        var user = await getUser();
        const url = `${BASE_URL}api/chat/${chatUUID}/1`;
        var rs = await getDataFromUrl(url);
        var tynChatBody = document.querySelector("#tynChatBody");
        var tynChatHead = document.querySelector(".tyn-chat-head");

        var imgTags = tynChatHead.querySelectorAll("img");
        var chatName = tynChatHead.querySelector("h6");
        var chatReply = document.querySelector("#tynReply");
        chatReply.innerHTML = "";
        chatReply.setAttribute('data-page-number', '1')

        if (!rs.isGroup) {
            if (rs.messageResponse.length <= 1) {
                imgTags.forEach(img => {
                    img.src = rs.avatarUrl;
                });
                chatName.textContent = rs.chatName;
            }
            for (let i = 0; i < rs.messageResponse.length; i++) {
                let message = rs.messageResponse[i];
                if (user.uuid != message.userResponse.userUUID) {
                    imgTags.forEach(img => {
                        img.src = message.userResponse.avatarUrl;
                    });
                    chatName.textContent = message.userResponse.nickName;
                    break;
                }
            }
        }
        if (rs.isGroup) {
            imgTags.forEach(img => {
                img.src = rs.avatarUrl;
            });
            chatName.textContent = rs.chatName;
        }

        let previousMessageTime = new Date();// current time
        //afterbegin , beforeend
        rs.messageResponse.forEach(message => {
            var contentMessage = ``;
            var IsIncoming = false;

            if (user.uuid == message.userResponse.userUUID) {
                if (message.contentType == "Text") {
                    contentMessage = `
                        <div class="tyn-reply-text">${message.content}</div>
                    `
                }
                if (message.contentType == "File") {
                    var images = ``;
                    var isImage = false;
                    message.fileResponse.forEach(fileRs => {
                        if (fileRs.type == "Video") {
                            contentMessage = createMessageVideo(fileRs.path);
                        }
                        if (fileRs.type == "Image") {
                            isImage = true;
                            images += createMessageImage(fileRs.path);
                        }
                        if (fileRs.type == "Document") {
                            contentMessage = createMessageDocument(fileRs.path, fileRs.name);
                        }
                    });
                    if (isImage) {
                        contentMessage =
                            `<div class="tyn-reply-media">
                                    ${images}
                            </div>
                        `
                    }
                }

            } else {
                IsIncoming = true;
                if (message.contentType == "Text") {
                    contentMessage = `
                        <div class="tyn-reply-text">${message.content}</div>
                    `
                }
                if (message.contentType == "File") {
                    var images = ``;
                    var isImage = false;
                    message.fileResponse.forEach(fileRs => {

                        if (fileRs.type == "Video") {
                            contentMessage = createMessageVideo(fileRs.path);
                        }
                        if (fileRs.type == "Image") {
                            isImage = true;
                            images += createMessageImage(fileRs.path);
                        }
                        if (fileRs.type == "Document") {
                            contentMessage = createMessageDocument(fileRs.path, fileRs.name);
                        }
                    });
                    if (isImage) {
                        contentMessage =
                            `<div class="tyn-reply-media">
                                    ${images}
                            </div>
                        `
                    }
                }

            }
            var chatIncoming = createChatIncoming(message.messageUUID, message.userResponse.avatarUrl, contentMessage, message.userResponse.nickName);
            var chatOutgoing = createChatOutgoing(message.messageUUID, contentMessage);

            if (IsIncoming) {
                chatReply.insertAdjacentHTML("beforeend", chatIncoming);
            } else {
                chatReply.insertAdjacentHTML("beforeend", chatOutgoing);
            }
            var timeView = `<div class="tyn-reply-separator">${formatDate(message.createdAt)}</div>`;
            let createdAtDate = new Date(message.createdAt);
            if (previousMessageTime - createdAtDate > (10 * 60 * 1000)) {
                previousMessageTime = new Date(message.createdAt);
                chatReply.insertAdjacentHTML("beforeend", timeView);
            }
        });


        refreshLightbox();
        messageAction();
        //chatReply.lastElementChild.scrollIntoView({ behavior: 'smooth' });
        //chatReply.firstElementChild.scrollIntoView({ behavior: 'smooth' });
        if (chatReply.firstElementChild != null) {
            chatReply.firstElementChild.scrollIntoView();
        }

    }

    function sendDataToChat(messageRequest) {
        var chatReply = document.querySelector("#tynReply");
        connection.invoke("SendDataToChat", messageRequest)
            .then((message) => {
                console.log(message);
                var contentMessage = ``;
                if (message.contentType == "Text") {
                    contentMessage = `
                        <div class="tyn-reply-text">${message.content}</div>
                    `
                }
                if (message.contentType === "File") {
                    var images = ``;
                    var isImage = false;
                    message.fileResponse.forEach(fileRs => {
                        if (fileRs.type === "Video") {
                            contentMessage = createMessageVideo(fileRs.path);
                        }
                        if (fileRs.type === "Image") {
                            isImage = true;
                            images += createMessageImage(fileRs.path);
                        }
                        if (fileRs.type === "Document") {
                            contentMessage = createMessageDocument(fileRs.path, fileRs.name);
                        }
                    });

                    if (isImage) {
                        contentMessage = `
                        <div class="tyn-reply-media">
                            ${images}
                        </div>
                    `;
                    }

                }
                var chatOutgoing = createChatOutgoing(message.messageUUID, contentMessage);
                chatReply.insertAdjacentHTML("afterbegin", chatOutgoing);

                refreshLightbox();
                messageAction();
                chatReply.firstElementChild.scrollIntoView();

            })
            .catch(err => {
                console.error(err.toString());
            });
    }

    //send file
    var documentSend = document.querySelector("#tyn-document-send");
    var videoSend = document.querySelector("#tyn-video-send")
    var imageSend = document.querySelector("#tyn-btn-img-send");

    var inputDocument = document.querySelector("#file-input-document")
    var inputVideo = document.querySelector("#file-input-video")
    var inputImage = document.querySelector("#file-input-img")

    documentSend.addEventListener("click", function () {
        inputDocument.click();
    });

    videoSend.addEventListener("click", function () {
        inputVideo.click();
    });

    imageSend.addEventListener("click", function () {
        inputImage.click();
    });

    //Đổi thành ChatUUID ở trên
    const messageRequest = {
        chatUUID: chatUUID,
        messageType: "",
        content: "",
        fileRequests: null
    };

    inputDocument.addEventListener("change", function () {
        var files = inputDocument.files;
        getListFile(files).then(fileRequests => {
            console.log(fileRequests)
            if (fileRequests.length > 0) {
                messageRequest.chatUUID = chatUUID;
                messageRequest.messageType = "Document"
                messageRequest.fileRequests = fileRequests;
                sendDataToChat(messageRequest);
            }
        })
    });

    inputVideo.addEventListener("change", function () {
        var files = inputVideo.files;
        getListFile(files).then(fileRequests => {
            console.log(fileRequests)
            if (fileRequests.length > 0) {
                messageRequest.chatUUID = chatUUID;
                messageRequest.messageType = "Video"
                messageRequest.fileRequests = fileRequests;
                sendDataToChat(messageRequest);
            }
        })
    });

    inputImage.addEventListener("change", function () {
        var files = inputImage.files;
        getListFile(files).then(fileRequests => {
            console.log(fileRequests)
            if (fileRequests.length > 0) {
                messageRequest.chatUUID = chatUUID;
                messageRequest.messageType = "Image"
                messageRequest.fileRequests = fileRequests;
                sendDataToChat(messageRequest);
            }
        })

    });

    let chatSend = document.querySelector('#tynChatSend');
    let chatInput = document.querySelector('#tynChatInput');
    var chatReply = document.querySelector("#tynReply");
    let chatBody = document.querySelector('#tynChatBody');
    chatSend && chatSend.addEventListener("click", function (event) {
        event.preventDefault();
        let getInput = chatInput.innerText;
        chatInput.innerHTML = "";

        messageRequest.chatUUID = chatUUID;
        messageRequest.messageType = 'Text';
        messageRequest.content = getInput;
        messageRequest.fileRequests = null;

        sendDataToChat(messageRequest)

    })

    chatInput && chatInput.addEventListener("keypress", function (event) {
        if (event.key === "Enter" && !event.shiftKey) {
            event.preventDefault()
            chatSend.click();
        }
    });
    function arrayBufferToBase64(buffer) {
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }
    const MAX_TOTAL_SIZE = 20 * 1024 * 1024;
    async function getListFile(files) {
        var fileRequests = [];
        var totalSize = 0;
        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            totalSize += file.size;
            if (totalSize > MAX_TOTAL_SIZE) {
                toastr.options = {
                    "closeButton": true,
                    "debug": false,
                    "newestOnTop": false,
                    "progressBar": true,
                    "positionClass": "toast-top-center",
                    "preventDuplicates": true,
                    "onclick": null,
                    "showDuration": "300",
                    "hideDuration": "1000",
                    "timeOut": "5000",
                    "extendedTimeOut": "1000",
                    "showEasing": "swing",
                    "hideEasing": "linear",
                    "showMethod": "fadeIn",
                    "hideMethod": "fadeOut"
                }

                toastr["warning"]("`The total file size exceeds the 20MB limit. Please select fewer files.", "Overload")
                return [];
            }
            console.log(file.type);
            console.log(file.size);

            var dataFile = await readFileAsArrayBuffer(file);
            var base64Data = arrayBufferToBase64(dataFile);
            var fileRequest = {
                name: file.name,
                datafile: base64Data
            };
            fileRequests.push(fileRequest);
        }
        return fileRequests;
    }
    function readFileAsArrayBuffer(file) {
        return new Promise((resolve, reject) => {
            var reader = new FileReader();
            reader.onload = function (event) {
                resolve(event.target.result);
            };
            reader.onerror = function (event) {
                reject(event.target.error);
            };
            reader.readAsArrayBuffer(file);
        });
    }

    connection.on("LoadMessageReceiveChat", (message, chatUUIDofMessage) => {
        if (chatUUID != chatUUIDofMessage) return;
        console.log(message);
        var contentMessage = ``;
        if (message.contentType == "Text") {
            contentMessage = `
                        <div class="tyn-reply-text">${message.content}</div>
                    `
        }
        if (message.contentType === "File") {
            var images = ``;
            var isImage = false;
            message.fileResponse.forEach(fileRs => {
                if (fileRs.type === "Video") {
                    contentMessage = createMessageVideo(fileRs.path);
                }
                if (fileRs.type === "Image") {
                    isImage = true;
                    images += createMessageImage(fileRs.path);
                }
                if (fileRs.type === "Document") {
                    contentMessage = createMessageDocument(fileRs.path, fileRs.name);
                }
            });

            if (isImage) {
                contentMessage = `
                        <div class="tyn-reply-media">
                            ${images}
                        </div>
                    `;
            }
        }
        var chatIncoming = createChatIncoming(message.messageUUID, message.userResponse.avatarUrl, contentMessage, message.userResponse.nickName);
        chatReply.insertAdjacentHTML("afterbegin", chatIncoming);
        messageAction();
        refreshLightbox();
        chatReply.firstElementChild.scrollIntoView();

    });
    messageAction();
    function messageAction() {
        //edit message
        const editMessages = document.querySelectorAll('.edit-message');
        editMessages.forEach(link => {
            link.addEventListener('click', function (event) {
                event.preventDefault();
                const messageUUID = this.getAttribute('data-message-uuid');

                const replyTextDiv = this.closest('.tyn-reply-bubble').querySelector('.tyn-reply-text');
                if (!replyTextDiv) return;

                const originalText = replyTextDiv.textContent;
                const input = document.createElement('input');
                input.type = 'text';
                input.value = originalText;
                input.className = 'tyn-reply-input';

                replyTextDiv.replaceWith(input);
                input.focus();

                let isEditing = false;

                function replaceWithDiv(newText) {
                    const newDiv = document.createElement('div');
                    newDiv.className = 'tyn-reply-text';
                    newDiv.textContent = newText;

                    input.replaceWith(newDiv);
                    messageAction();
                }
                function editMessage() {
                    if (isEditing) return;
                    isEditing = true;
                    const newText = input.value;
                    connection.invoke("EditMessageInChat", newText, messageUUID, chatUUID)
                        .then((status) => {
                            if (status) {
                                replaceWithDiv(newText);
                            } else {
                                toastr.options = {
                                    "closeButton": true,
                                    "debug": false,
                                    "newestOnTop": true,
                                    "progressBar": true,
                                    "positionClass": "toast-top-center",
                                    "preventDuplicates": true,
                                    "onclick": null,
                                    "showDuration": "300",
                                    "hideDuration": "1000",
                                    "timeOut": "5000",
                                    "extendedTimeOut": "1000",
                                    "showEasing": "swing",
                                    "hideEasing": "linear",
                                    "showMethod": "fadeIn",
                                    "hideMethod": "fadeOut"
                                }
                                toastr["error"]("You can only edit messages within the last 1 hour.", "Edit failed")
                                replaceWithDiv(originalText);
                            }
                        })
                        .catch((err) => {
                            console.error("Error editing message: ", err.toString());
                            replaceWithDiv(originalText);
                        })
                        .finally(() => {
                            isEditing = false;
                        });
                }

                input.addEventListener('keypress', function (event) {
                    if (event.key === 'Enter') {
                        editMessage();
                    }
                });

                input.addEventListener('blur', function () {
                    editMessage();
                });
            });
        });

        //remove message
        const deleteMessages = document.querySelectorAll('.delete-message');
        deleteMessages.forEach(link => {
            link.addEventListener('click', function (event) {
                event.preventDefault();
                const messageUUID = this.getAttribute('data-message-uuid');

                //để chatUUID vào
                connection.invoke("DeleteMessageInChat", messageUUID, chatUUID)
                    .then((status) => {
                        if (status) {
                            const messageElement = document.getElementById(messageUUID);
                            const nextElement = messageElement.nextElementSibling;
                            if (nextElement && nextElement.classList.contains('tyn-reply-separator')) {
                                nextElement.remove();
                            }
                            messageElement.remove();
                        } else {
                            toastr.options = {
                                "closeButton": true,
                                "debug": false,
                                "newestOnTop": true,
                                "progressBar": true,
                                "positionClass": "toast-top-center",
                                "preventDuplicates": true,
                                "onclick": null,
                                "showDuration": "300",
                                "hideDuration": "1000",
                                "timeOut": "5000",
                                "extendedTimeOut": "1000",
                                "showEasing": "swing",
                                "hideEasing": "linear",
                                "showMethod": "fadeIn",
                                "hideMethod": "fadeOut"
                            }
                            toastr["error"]("You can only delete messages within the last 1 hour.", "Delete failed")
                        }
                    })
                    .catch(() => {
                        alert("Error deleting message");
                    });
            });
        });
    }

    connection.on("LoadMessageEdit", (contentText, messageUUID) => {
        const messageElement = document.getElementById(messageUUID);
        if (messageElement) {
            const messageText = messageElement.querySelector(".tyn-reply-text");
            messageText.textContent = contentText;
        }
    });

    connection.on("LoadMessageDelete", (messageUUID) => {
        const messageElement = document.getElementById(messageUUID);
        if (messageElement != null) {
            const nextElement = messageElement.nextElementSibling;
            if (nextElement && nextElement.classList.contains('tyn-reply-separator')) {
                nextElement.remove();
            }
            messageElement.remove();
        }
    });
    
    // load lazy message chat
    async function loadLazyMessage(element) {
        var user = await getUser();
        var chatReply = document.querySelector("#tynReply");
        let pageNumberStr = chatReply.getAttribute('data-page-number');
        let pageNumber = parseInt(pageNumberStr, 10);
        console.log("loadLazyMessage pagenumber: " + pageNumber);
        if (element == null) return;
        const url = `${BASE_URL}api/chat/${chatUUID}/${pageNumber + 1}`;
        var rs = await getDataFromUrl(url);
        if (!rs.messageResponse || rs.messageResponse.length === 0) {
            return
        }
        chatReply.setAttribute('data-page-number', pageNumber + 1)

        var currentMessageID = null;

        let previousMessageTime = new Date();// current time
        //afterbegin , beforeend
        rs.messageResponse.forEach(message => {
            var contentMessage = ``;
            var IsIncoming = false;

            if (user.uuid == message.userResponse.userUUID) {
                if (message.contentType == "Text") {
                    contentMessage = `
                        <div class="tyn-reply-text">${message.content}</div>
                    `
                }
                if (message.contentType == "File") {
                    var images = ``;
                    var isImage = false;
                    message.fileResponse.forEach(fileRs => {
                        if (fileRs.type == "Video") {
                            contentMessage = createMessageVideo(fileRs.path);
                        }
                        if (fileRs.type == "Image") {
                            isImage = true;
                            images += createMessageImage(fileRs.path);
                        }
                        if (fileRs.type == "Document") {
                            contentMessage = createMessageDocument(fileRs.path, fileRs.name);
                        }
                    });
                    if (isImage) {
                        contentMessage =
                            `<div class="tyn-reply-media">
                                    ${images}
                            </div>
                        `
                    }
                }

            } else {
                IsIncoming = true;
                if (message.contentType == "Text") {
                    contentMessage = `
                        <div class="tyn-reply-text">${message.content}</div>
                    `
                }
                if (message.contentType == "File") {
                    var images = ``;
                    var isImage = false;
                    message.fileResponse.forEach(fileRs => {

                        if (fileRs.type == "Video") {
                            contentMessage = createMessageVideo(fileRs.path);
                        }
                        if (fileRs.type == "Image") {
                            isImage = true;
                            images += createMessageImage(fileRs.path);
                        }
                        if (fileRs.type == "Document") {
                            contentMessage = createMessageDocument(fileRs.path, fileRs.name);
                        }
                    });
                    if (isImage) {
                        contentMessage =
                            `<div class="tyn-reply-media">
                                    ${images}
                            </div>
                        `
                    }
                }

            }
            var chatIncoming = createChatIncoming(message.messageUUID, message.userResponse.avatarUrl, contentMessage, message.userResponse.nickName);
            var chatOutgoing = createChatOutgoing(message.messageUUID, contentMessage);

            if (IsIncoming) {
                chatReply.insertAdjacentHTML("beforeend", chatIncoming);
            } else {
                chatReply.insertAdjacentHTML("beforeend", chatOutgoing);
            }
            var timeView = `<div class="tyn-reply-separator">${formatDate(message.createdAt)}</div>`;
            let createdAtDate = new Date(message.createdAt);
            if (previousMessageTime - createdAtDate > (10 * 60 * 1000)) {
                previousMessageTime = new Date(message.createdAt);
                chatReply.insertAdjacentHTML("beforeend", timeView);
            }
            if (currentMessageID == null) {
                currentMessageID = message.messageUUID;
            }
        });


        refreshLightbox();
        messageAction();
        let oldMessage = document.getElementById(currentMessageID);
        if (oldMessage) {
            oldMessage.scrollIntoView({ behavior: 'auto', block: 'start' });
        }
    }

    //search message in chat
    searchMessage();
    var currentIndexSearch = 0;
    var dictionary = null;
    async function searchMessage() {
        const searchInThisChat = document.querySelector("#searchInThisChat");
        const aboveMessage = document.querySelector("#above-message");
        const belowMessage = document.querySelector("#below-message");
        var messageOld = null;
        var messageOldCurrentBackgroundColor = null;
        searchInThisChat.addEventListener('keydown', async function (event) {
            if (event.key === 'Enter') {
                const url = `${BASE_URL}api/chat/search/${chatUUID}/${searchInThisChat.value}`;
                var rs = await getDataFromUrl(url);
                if (Object.keys(rs).length === 0) {
                    return; 
                }
                dictionary = rs;
                console.log(dictionary);
                aboveMessage.click();
            }
        });

        aboveMessage.addEventListener("click", async () => {
            if (dictionary == null || lastClickedElement == null) return;
            var chatReply = document.querySelector("#tynReply");
            let pageNumberStr = chatReply.getAttribute('data-page-number');
            let pageNumber = parseInt(pageNumberStr, 10);
            const entries = Object.entries(dictionary);

            // Đặt lại màu nền cũ nếu có
            if (messageOld != null) {
                messageOld.style.backgroundColor = `${messageOldCurrentBackgroundColor}`;
            }

            var messageIndex = 0;
            console.log("currentIndexSearch: " + currentIndexSearch)
            for (const [key, value] of entries) {
                for (const messageUUID of value) {
                    messageIndex += 1;
                    while (true) {
                        pageNumberStr = chatReply.getAttribute('data-page-number');
                        pageNumber = parseInt(pageNumberStr, 10);
                        await loadLazyMessage(lastClickedElement);
                        if (pageNumber >= key) {
                            break;
                        }
                    }
                    console.log('#' + messageUUID)
                    const messageElement = document.querySelector('#' + messageUUID);
                    if (messageElement != null && messageIndex == currentIndexSearch + 1) {
                        currentIndexSearch += 1;
                        var messageText = messageElement.querySelector(".tyn-reply-text");
                        messageOld = messageText;
                        messageOldCurrentBackgroundColor = window.getComputedStyle(messageText).backgroundColor;

                        // Đặt màu nền mới
                        messageText.style.backgroundColor = '#ffc107';
                        messageElement.scrollIntoView({ behavior: 'auto', block: 'center' });
                        return;
                    }
                }
            }
        });


        belowMessage.addEventListener("click", () => {
            if (dictionary == null) return;
            const entries = Object.entries(dictionary);
            if (messageOld != null) {
                messageOld.style.backgroundColor = `${messageOldCurrentBackgroundColor}`;
            }

            var messageIndex = 0;
            for (const [key, value] of entries) {
                for (const messageUUID of value) {
                    messageIndex += 1;
                    const messageElement = document.querySelector('#' + messageUUID);

                    if (messageElement != null && messageIndex == currentIndexSearch - 1) {
                        currentIndexSearch -= 1;
                        var messageText = messageElement.querySelector(".tyn-reply-text");
                        messageOld = messageText;
                        messageOldCurrentBackgroundColor = window.getComputedStyle(messageText).backgroundColor;

                        messageText.style.backgroundColor = '#ffc107';
                        messageElement.scrollIntoView({ behavior: 'auto', block: 'center' });
                        return;
                    }
                }
            }
        });

        var tynChatSearch = document.querySelector("#tynChatSearch");
        var observer = new MutationObserver(function (mutationsList) {
            for (var mutation of mutationsList) {
                if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                    if (!tynChatSearch.classList.contains('active')) {
                        handleActiveRemoved();
                    }
                }
            }
        });

        observer.observe(tynChatSearch, { attributes: true, subtree: true });
        function handleActiveRemoved() {
            const searchInThisChat = document.querySelector("#searchInThisChat");
            searchInThisChat.value = "";
            currentIndexSearch = 0;
            dictionary = null;
            if (messageOld != null) {
                messageOld.style.backgroundColor = `${messageOldCurrentBackgroundColor}`;
            }
        }
    }

    //trạng thái nhóm
    var statusChat = document.querySelector("#status-chat")
    function onStatusChat() {
        connection.on("OnStatusChat", (_chatUUID, status) => {
            if (chatUUID != null && _chatUUID == chatUUID) {
                if (status) {
                    statusChat.innerHTML = 'Active';
                    statusChat.style.color = 'green';
                } else {
                    statusChat.textContent = 'InActive';
                    statusChat.style.color = 'Red';
                }
            }
        });
        connection.invoke("IsGroupEmpty", chatUUID)
            .then((status) => {
                if (!status) {
                    statusChat.innerHTML = 'Active';
                    statusChat.style.color = 'green';
                } else {
                    statusChat.textContent = 'InActive';
                    statusChat.style.color = 'Red';
                }
            }).catch(err => {
                console.error(err.toString());
            });
    }
});