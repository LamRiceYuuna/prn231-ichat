document.addEventListener('DOMContentLoaded', function () {


    // Xử lý sự kiện khi nút "Add Friends to Group" được nhấn
    document.querySelector('[data-bs-toggle="modal"][data-bs-target="#addFriendsModal"]').addEventListener('click', async function () {

        const token = await getToken(); 

        if (!token) {
            console.error("Token is required to fetch friends.");
            return;
        }

        const apiUrl = `${BASE_URL}api/FriendShip/friends_not_in_chat/${chatUUID}`;

        try {
            const response = await fetch(apiUrl, {
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
                throw new Error("Network response was not ok: " + response.statusText);
            }

            const data = await response.json();
            console.log('Data from API:', data);

            const friendsList = document.getElementById('friendsList');
            friendsList.innerHTML = ''; // Xóa danh sách hiện tại

            // Giả sử data là mảng các bạn bè
            data.forEach(friend => {
                const listItem = document.createElement('li');
                listItem.className = 'list-group-item';
                listItem.innerHTML = `
                    <div class="d-flex align-items-center">
                        <img src="${friend.avatarUrl}" alt="${friend.nickName}" class="rounded-circle me-3" width="40" height="40">
                        <div class="d-flex justify-content-between align-items-center w-100">
                            <span>${friend.nickName}</span>
                            <button class="btn btn-primary btn-sm" data-friend-id="${friend.userId}">Add</button>
                        </div>
                    </div>
                `;
                friendsList.appendChild(listItem);
            });

        } catch (error) {
            console.error('Error fetching data:', error);
        }
    });

    document.addEventListener('click', function (event) {
        if (event.target.matches('#friendsList .btn-primary')) {
            const button = event.target;

            // Chuyển trạng thái giữa "Add" và "Added"
            if (button.textContent === 'Add') {
                button.textContent = 'Added';
                button.classList.add('btn-added');
            } else {
                button.textContent = 'Add';
                button.classList.remove('btn-added');
            }
        }
    });

    const searchButton = document.getElementById('searchButton');

    function filterFriends() {
        const searchInput = document.getElementById('searchFriends');
        const friendsList = document.getElementById('friendsList');
        const friendsItems = friendsList.querySelectorAll('.list-group-item');

        const searchTerm = searchInput.value.toLowerCase();

        friendsItems.forEach(item => {
            const friendNameElement = item.querySelector('span');

            // Kiểm tra xem phần tử tên bạn bè có tồn tại không
            if (friendNameElement) {
                console.log("friendName:", friendNameElement); // Kiểm tra từ khóa tìm kiếm

               const friendName = friendNameElement.textContent.trim().toLowerCase();

                // Hiển thị hoặc ẩn mục bạn bè dựa trên từ khóa tìm kiếm
                console.log("friendName:", friendName); // Kiểm tra từ khóa tìm kiếm
                console.log("searchTerm:", searchTerm); // Kiểm tra từ khóa tìm kiếm

                if (friendName.includes(searchTerm)) {
                    item.style.display = ''; // Hiển thị mục
                } else {
                    item.style.display = 'none'; // Ẩn mục
                }
            }
        });
    }

    // Xử lý sự kiện khi người dùng nhấn nút tìm kiếm
    searchButton.addEventListener('click', filterFriends);

    // Lắng nghe sự kiện khi nhấn nút "Add Selected Friends"
    document.getElementById('addSelectedFriends').addEventListener('click', async function () {
        // Lấy các nút "Add" từ danh sách bạn bè
        const addButtons = document.querySelectorAll('#friendsList .btn-primary');
        const selectedFriendIds = [];

        // Duyệt qua tất cả các nút và lấy các ID của bạn bè đã chọn
        addButtons.forEach(button => {
            if (button.classList.contains('btn-added')) {
                selectedFriendIds.push(button.getAttribute('data-friend-id'));
            }
        });

        // Nếu không có bạn bè nào được chọn, thông báo cho người dùng
        if (selectedFriendIds.length === 0) {
            alert('Please select at least one friend to add.');
            return;
        }

        console.log("selectedFriendIds", selectedFriendIds);

        const apiUrl = `${BASE_URL}api/Chat/add_friends_to_group/${chatUUID}`;

        // Gửi yêu cầu API để thêm bạn bè vào nhóm
        try {
            const response = await fetch(apiUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify(selectedFriendIds)
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            console.log('Successfully added friends:', data);
            // Có thể thêm các hành động sau khi thêm bạn bè thành công (như đóng modal, làm mới danh sách, v.v.)
            $('#addFriendsModal').modal('hide'); // Đóng modal
        } catch (error) {
            console.error('Error adding friends:', error);
        }
    });

});
