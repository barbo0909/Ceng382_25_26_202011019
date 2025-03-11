

const users = []; 

document.addEventListener("DOMContentLoaded", function () {
    const loginButton = document.querySelector(".login-btn");

    loginButton.addEventListener("click", function(event) {
        event.preventDefault(); 

        const username = document.getElementById("username").value;
        const password = document.getElementById("password").value;

        if (username && password) {
           
            if (username === "admin" && password === "admin") {
               
                window.location.href = "table.html";
            } else {
                console.log("Hatalı kullanıcı adı veya şifre!"); 
            }

            users.push({ username, password }); 
            console.clear();
            console.log("Kullanıcılar:");
            users.forEach((user, index) => {
                console.log(`${index + 1}. Username: ${user.username}, Password: ${user.password}`);
            });

         
            document.getElementById("username").value = "";
            document.getElementById("password").value = "";
        } else {
            console.log("Lütfen her iki alanı da doldurun.");
        }
    });
});
