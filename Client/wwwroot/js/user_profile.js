
async function loadUserProfile() {
    const token = await getToken();
    try {
        const response = await fetch('https://localhost:7119/api/User/profile', {
        method: 'GET',
    headers: {
        'Accept': 'application/json',
    'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
            }
        });

    if (!response.ok) {
            throw new Error('Network response was not ok ' + response.statusText);
        }

    const data = await response.json();

    // Cập nhật giao diện với dữ liệu nhận được
    document.getElementById('UserName').value = data.userName;
    document.getElementById('NickName').value = data.nickName;
    document.getElementById('primaryEmail').value = data.email;
    window.uuid = data.uuid;
    window.userId = data.userId;
    window.profileId = data.profileId;
        window.password = data.password;
    window.avatarUrl = data.avatarUrl;
    window.hasPassword = data.hasPassword;
    } catch (error) {
        console.error('Error fetching user profile:', error);
    }
}

    // Gọi hàm loadUserProfile khi trang được tải
    document.addEventListener('DOMContentLoaded', loadUserProfile);

    // Hàm để lưu thông tin người dùng khi nhấn nút Save
async function saveUserProfile() {
    const token = await getToken();
   

    try {
        const newPassword = document.getElementById('NewPassWord').value;
        const confirmNewPassword = document.getElementById('ConfirmNewPassWord').value;
        if (newPassword !== confirmNewPassword) {
            toastr.warning('The new password and the authentication password do not match')
            return;
        }

    const userProfile = {
    userId: window.userId,
    uuid: window.uuid,
    userName: document.getElementById('UserName').value,
    email: document.getElementById('primaryEmail').value,
    password: newPassword ? newPassword : window.password,
    hasPassword: window.hasPassword,
    profileId: window.profileId,
    nickName: document.getElementById('NickName').value,
    avatarUrl: window.avatarUrl
        };
    const response = await fetch('https://localhost:7119/api/User/profile', {
        method: 'Put',
    headers: {
        'Accept': 'application/json',
    'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
            },
    body: JSON.stringify(userProfile)
        });
    if (!response.ok) {
        toastr.warning('Network response was not ok ')
        }
        const data = await response.json();
        toastr.success('User profile saved successfully!', 'successfully')
    } catch (error) {
        toastr.warning('Error saving user profile')
    }
}