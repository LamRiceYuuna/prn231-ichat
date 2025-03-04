document.addEventListener("DOMContentLoaded", function () {
    const radioButtons = document.querySelectorAll('input[name="muteFor"]');
    const muteButton = document.querySelector('.js-chat-mute-toggle');
    const BASE_API_URL = BASE_URL + 'api/ChatMember/';

    // Bắt sự kiện thay đổi của radio button
    radioButtons.forEach(radioButton => {
        radioButton.addEventListener('change', function () {
            if (this.checked) {
                let muteDuration = 0;

                switch (this.id) {
                    case 'muteFor15min':
                        muteDuration = 15;
                        break;
                    case 'muteFor1Hour':
                        muteDuration = 60;
                        break;
                    case 'muteFor1Days':
                        muteDuration = 1440;
                        break;
                    case 'muteForInfinity':
                        muteDuration = 0;
                        break;
                    default:
                        return; // Trường hợp không xác định, không làm gì
                }

                if (muteDuration >= 0) {
                    muteChatMember(muteDuration);
                }
            }
        });
    });

    // Bắt sự kiện click vào nút Mute/Unmute
    muteButton.addEventListener('click', async function () {

        if (muteButton.classList.contains('chat-muted')) {

            unmuteChatMember();
        } else {

            muteChatMember(0); // Mute vĩnh viễn khi click vào Mute
        }
    });

    // Hàm gọi API để Mute hoặc cập nhật thời gian Mute
    function muteChatMember(muteDuration) {
        const apiUrl = `${BASE_API_URL}MuteChatMember/${chatMemberId}/${muteDuration}`;

        callApi(apiUrl, 'POST', muteDuration === 0 ? 'Unmuted successfully test' : `Muted for ${muteDuration} minutes successfully`);
    }

    // Hàm gọi API để Unmute
    function unmuteChatMember() {
        const apiUrl = `${BASE_API_URL}UnmuteChatMember/${chatMemberId}`;

        callApi(apiUrl, 'POST', 'Unmuted successfully');
    }

    // Hàm gọi API chung
    function callApi(apiUrl, method, successMessage) {

        fetch(apiUrl, {
            method: method,
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (response.ok) {
                    console.log(successMessage);
                } else {
                    throw new Error('Operation failed');
                }
            })
            .catch(error => console.error('Error:', error));
    }
});
