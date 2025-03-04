// Khai báo các biến toàn cục để lấy các phần tử DOM
let uploadButton, uploadModal, closeButton, fileInput, uploadFileButton, preview;

// Sự kiện khi tài liệu được tải xong
document.addEventListener('DOMContentLoaded', function () {
    // Lấy các phần tử DOM từ HTML
    uploadButton = document.getElementById('uploadButton');
    uploadModal = document.getElementById('uploadModal');
    closeButton = document.querySelector('.close');
    fileInput = document.getElementById('fileInput');
    uploadFileButton = document.getElementById('uploadFileButton');
    preview = document.getElementById('preview');

    // Thêm các sự kiện vào các phần tử DOM
    uploadButton.addEventListener('click', function () {
        uploadModal.style.display = 'block';
    });

    closeButton.addEventListener('click', function () {
        uploadModal.style.display = 'none';
    });

    window.addEventListener('click', function (event) {
        if (event.target === uploadModal) {
            uploadModal.style.display = 'none';
        }
    });

    fileInput.addEventListener('change', function () {
        const file = fileInput.files[0];
        preview.innerHTML = '';

        if (file) {
            const fileType = file.type;
            const validImageTypes = ['image/gif', 'image/jpeg', 'image/png'];
            const validVideoTypes = ['video/mp4', 'video/webm', 'video/ogg'];

            if (validImageTypes.includes(fileType)) {
                const img = document.createElement('img');
                img.src = URL.createObjectURL(file);
                img.style.maxWidth = '100%';
                preview.appendChild(img);
            } else if (validVideoTypes.includes(fileType)) {
                const video = document.createElement('video');
                video.src = URL.createObjectURL(file);
                video.controls = true;
                video.style.maxWidth = '100%';
                preview.appendChild(video);
            } else {
                preview.innerText = 'File không hợp lệ. Vui lòng chọn ảnh hoặc video.';
            }
        }
    });

    uploadFileButton.addEventListener('click', async function () {
        const file = fileInput.files[0];
        if (!file) {
            alert('Please select a file.');
            return;
        }

        // Kiểm tra kích thước tệp
        if (file.size > 10 * 1024 * 1024) { // 10 MB
            alert('File size exceeds 10MB.');
            return;
        }

        // Lấy token từ hàm getToken (đảm bảo hàm này là một async function)
        const token = await getToken();

        if (!token) {
            alert('Failed to get token.');
            return;
        }

        // Gửi dữ liệu lên server
        const formData = new FormData();
        formData.append('file', file);

        fetch(BASE_URL + 'api/Story/CreateStory', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`
            },
            body: formData
        })
            .then(response => response.json().catch(() => {
                throw new Error('Invalid JSON response');
            }))
            .then(data => {
                console.log('File uploaded successfully:', data);
                uploadModal.style.display = 'none';

                // Gọi lại API để cập nhật danh sách stories
                updateStories();
            })
            .catch(error => {
                console.error('Error uploading file:', error);
                alert(`Error uploading file: ${error.message}`);
            });
    });

    // Hàm cập nhật danh sách stories
    async function updateStories() {
        const storiesWrapperTop = document.querySelector('#stories-wrapper');
        const storiesWrapperBottom = document.querySelector('.tyn-stories-slider .swiper-wrapper');

        try {
            const user = await getUser();
            const response = await fetch(`https://localhost:7119/api/Story/${user.uuid}`);
            const stories = await response.json();

            storiesWrapperTop.innerHTML = ''; // Xóa nội dung cũ
            storiesWrapperBottom.innerHTML = ''; // Xóa nội dung cũ

            stories.forEach(story => {
                console.log(story);

                // Đối với div ở trên
                const storyItemTop = document.createElement('div');
                storyItemTop.classList.add('swiper-slide');
                storyItemTop.innerHTML = `
    <div style="position: relative;">
        <img src="${story.path}" class="tyn-image" alt="">
        <button class="delete-button" onclick="deleteStory('${story.storyId}')">X</button>
    </div>
`;

                storiesWrapperTop.appendChild(storyItemTop);

                // Đối với div ở dưới
                const storyItemBottom = document.createElement('div');
                storyItemBottom.classList.add('swiper-slide');
                storyItemBottom.innerHTML = `
                    <div class="tyn-stories-item">
                        <img src="${story.path}" class="tyn-image1" alt="">
                    </div>
                `;
                storiesWrapperBottom.appendChild(storyItemBottom);
            });
        } catch (error) {
            console.error('Error fetching stories:', error);
        }
    }

    // Hàm xóa story
    window.deleteStory = async function (id) { // Đảm bảo hàm deleteStory có thể được gọi từ toàn cục
        const token = await getToken();

        if (!token) {
            alert('Failed to get token.');
            return;
        }
        console.log(id,123);
        try {
            const response = await fetch(`https://localhost:7119/api/Story/DeleteStory?id=${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Failed to delete story: ${errorText}`);
            }

            alert('Story deleted successfully');
            updateStories();
        } catch (error) {
            console.error('Error deleting story:', error);
            alert(`Error deleting story: ${error.message}`);
        }
    };

    // Gọi hàm updateStories khi tài liệu được tải xong
    updateStories();
});
