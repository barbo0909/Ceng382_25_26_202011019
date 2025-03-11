document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("classForm");
    const classNameInput = document.getElementById("className");
    const numPeopleInput = document.getElementById("numPeople");
    const descriptionInput = document.getElementById("description");
    const tableBody = document.querySelector("#classTable tbody");

    let classes = [];

    
    form.addEventListener("submit", (event) => {
        event.preventDefault();

        const className = classNameInput.value;
        const numPeople = numPeopleInput.value;
        const description = descriptionInput.value;

        const newClass = { className, numPeople: parseInt(numPeople), description };
        classes.push(newClass);

        updateTable();

        
        alert("Class information added successfully!");

        form.reset();
    });

    
    function updateTable() {
        tableBody.innerHTML = "";

        classes.forEach((classInfo, index) => {
            const row = document.createElement("tr");

            row.innerHTML = `
              <td>${classInfo.className}</td>
              <td>${classInfo.numPeople}</td>
              <td>${classInfo.description}</td>
            `;

            
            row.addEventListener("click", () => {
                row.classList.toggle("highlight");
                console.log(`Row clicked: ${classInfo.className}, ${classInfo.numPeople}, ${classInfo.description}`);
            });

            
            row.addEventListener("mouseover", () => {
                row.style.backgroundColor = "#f1f1f1";
            });
            row.addEventListener("mouseout", () => {
                row.style.backgroundColor = "";
            });

           
            row.addEventListener("dblclick", () => {
                classes.splice(index, 1);
                updateTable();
            });

            tableBody.appendChild(row);
        });

        
        document.querySelector("#classTable").addEventListener("click", (event) => {
            if (!event.target.closest("tr")) {
                console.log(classes);
            }
        });
    }

   
    classNameInput.addEventListener("focus", () => {
        classNameInput.style.border = "2px solid blue";
    });
    classNameInput.addEventListener("blur", () => {
        classNameInput.style.border = "";
    });

    numPeopleInput.addEventListener("focus", () => {
        numPeopleInput.style.border = "2px solid blue";
    });
    numPeopleInput.addEventListener("blur", () => {
        numPeopleInput.style.border = "";
    });

    descriptionInput.addEventListener("focus", () => {
        descriptionInput.style.border = "2px solid blue";
    });
    descriptionInput.addEventListener("blur", () => {
        descriptionInput.style.border = "";
    });

    
    classNameInput.addEventListener("keyup", () => {
        if (classNameInput.value.length < 3) {
            classNameInput.setCustomValidity("Class name must be at least 3 characters long.");
        } else {
            classNameInput.setCustomValidity("");
        }
    });

    numPeopleInput.addEventListener("keyup", () => {
        if (numPeopleInput.value < 1) {
            numPeopleInput.setCustomValidity("Number of people must be a positive number.");
        } else {
            numPeopleInput.setCustomValidity("");
        }
    });

    descriptionInput.addEventListener("keyup", () => {
        if (descriptionInput.value.length < 5) {
            descriptionInput.setCustomValidity("Description must be at least 5 characters long.");
        } else {
            descriptionInput.setCustomValidity("");
        }
    });

   
    const tableRows = document.querySelectorAll("#classTable tr");
    tableRows.forEach((row) => {
        row.addEventListener("mouseover", () => {
            row.style.backgroundColor = "#f1f1f1"; 
        });
        row.addEventListener("mouseout", () => {
            row.style.backgroundColor = ""; 
        });
    });

    
    tableBody.addEventListener("click", (event) => {
        if (event.target.closest("tr")) {
            const row = event.target.closest("tr");
            row.classList.toggle("highlight"); 
            console.log(`Row clicked: ${row.innerHTML}`);
        }
    });

  
    document.querySelector("#classTable").addEventListener("click", (event) => {
        if (!event.target.closest("tr")) {
            console.log(classes); 
        }
    });

  
    const numPeopleHeader = document.querySelector("#classTable th:nth-child(2)");

    numPeopleHeader.addEventListener("click", () => {
        classes.sort((a, b) => b.numPeople - a.numPeople); 
        updateTable(); 
    });

  
    const classNameHeader = document.querySelector("#classTable th:nth-child(1)");

    classNameHeader.addEventListener("click", () => {
        classes.sort((a, b) => a.className.localeCompare(b.className)); 
        updateTable(); 
        toggleHeaderColor(classNameHeader); 
    });

   
    function toggleHeaderColor(header) {
       
        const headers = document.querySelectorAll("th");
        headers.forEach((header) => {
            header.style.backgroundColor = "";
        });

       
        header.style.backgroundColor = "#ddd";
    }
});
