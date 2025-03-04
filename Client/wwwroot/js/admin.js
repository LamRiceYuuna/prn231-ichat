document.addEventListener("DOMContentLoaded", async () => {

    var tbStatistic = document.getElementById('user-statistic');
    await populateUserStatisticsTable();
    async function populateUserStatisticsTable() {
        const url = `${BASE_URL}api/Statistic/load`
        var rs = await getDataFromUrl(url);
        const userStatistic = tbStatistic.getElementsByTagName('tbody')[0];
        rs.forEach(s => {
            const row = userStatistic.insertRow();

            const cellUuid = row.insertCell(0);
            const cellUsername = row.insertCell(1);
            const cellNickname = row.insertCell(2);
            const cellEmail = row.insertCell(3);
            const cellLastLogin = row.insertCell(4);
            const cellAccessTime = row.insertCell(5);

            cellUuid.textContent = s.uuid;
            cellUsername.textContent = s.username;
            cellNickname.textContent = s.nickName;
            cellEmail.textContent = s.email;
            cellLastLogin.textContent = new Date(s.lastLogin).toLocaleString();
            cellAccessTime.textContent = s.accessTimeFormatted;
        });
    }
    var tbUserBand = document.getElementById('user-band');
    await populateUserBand();
    async function populateUserBand() {
        const url = `${BASE_URL}api/User/load`
        var rs = await getDataFromUrl(url);

        const userBand = tbUserBand.getElementsByTagName('tbody')[0];
        rs.forEach(user => {
            const row = userBand.insertRow();

            const cellUuid = row.insertCell(0);
            const cellNickname = row.insertCell(1);
            const cellUsername = row.insertCell(2);
            const cellEmail = row.insertCell(3);
            const cellIsEmailConfirmed = row.insertCell(4);
            const cellHasPassword = row.insertCell(5);
            const cellLastLogin = row.insertCell(6);
            const cellRole = row.insertCell(7);
            const cellAction = row.insertCell(8);

            cellUuid.textContent = user.uuid;
            cellNickname.textContent = user.nickName;
            cellUsername.textContent = user.username;
            cellEmail.textContent = user.email;
            cellIsEmailConfirmed.textContent = user.isEmailConfirmed ? "Yes" : "No";
            cellHasPassword.textContent = user.hasPassword ? "Yes" : "No";
            cellLastLogin.textContent = new Date(user.lastLogin).toLocaleString();
            cellRole.textContent = user.role;


            const button = document.createElement('button');
            button.type = 'button';
            button.setAttribute('rel', 'tooltip');
            button.setAttribute('title', 'Band User');
            button.className = 'btn btn-danger btn-simple btn-xs';

            const icon = document.createElement('i');
            icon.className = 'fa fa-ban';

            button.appendChild(icon);
            if (user.status == 'Banned') {
                button.appendChild(document.createTextNode(' Banned'));
            } else {
                button.appendChild(document.createTextNode(' Band'));
            }
            

            button.addEventListener('click', () => {
                if (button.textContent.trim() === 'Band') {
                    banUser(user.uuid);
                    button.textContent = ' Banned';
                    button.className = 'btn btn-secondary btn-simple btn-xs';
                    icon.className = 'fa fa-check';
                } else {
                    unbanUser(user.uuid);
                    button.textContent = ' Band';
                    button.className = 'btn btn-danger btn-simple btn-xs';
                    icon.className = 'fa fa-ban';
                }
            });

            const td = document.createElement('td');
            td.className = 'td-actions text-right';
            td.appendChild(button);

            row.appendChild(td);

        });
    }
    async function fetchPutData(url, method, body = null) {
        const token = await getToken();
        try {
            const options = {
                method,
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            };

            if (body) {
                options.body = JSON.stringify(body);
            }

            const response = await fetch(url, options);

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
    async function banUser(uuid) {
        const url = `${BASE_URL}api/User/band-user?uuid=${uuid}`;
        return fetchPutData(url, 'PUT');
    }

    async function unbanUser(uuid) {
        const url = `${BASE_URL}api/User/band-user?uuid=${uuid}`;
        return fetchPutData(url, 'PUT');
    }

    async function getDataFromUrl(url) {
        return fetchPutData(url, 'GET');
    }

    var logout = document.querySelector('#log-out');
    logout.addEventListener('click', function (event) {
        event.preventDefault();
        document.cookie = 'bearer_token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
        setTimeout(function () {
            window.location.href = '/';
        }, 300);
    });
});