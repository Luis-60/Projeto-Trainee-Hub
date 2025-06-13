// Progress Bar
document.addEventListener('DOMContentLoaded', function() {
    // Seleciona todos os spans dentro de .progressbar
    document.querySelectorAll('.progressbar span').forEach(function(span) {
        // Pega o valor de data-width do span
        const progressWidth = span.getAttribute('data-width');
        
        // Define a largura do span com base no valor do data-width
        span.style.width = progressWidth + '%';
    });
});

document.querySelectorAll('.progressbar span').forEach(function(span) {
    // Pega o valor de data-width do span
    const progressWidth = span.getAttribute('data-width');
    
    // Define a largura do span com base no valor do data-width
    span.style.width = progressWidth + '%';
});


// Accordion
document.addEventListener("DOMContentLoaded", function () {
    const menuBtn = document.querySelector(".menu");
    const offScreenMenu = document.querySelector(".off-screen-menu");
    const overlay = document.querySelector(".overlay");

    menuBtn.addEventListener("click", function () {
        offScreenMenu.classList.toggle("active");
        menuBtn.classList.toggle("active");
        overlay.classList.toggle("active"); // Ativa/desativa o overlay
    });

    // Fecha o menu ao clicar no overlay
    overlay.addEventListener("click", function () {
        offScreenMenu.classList.remove("active");
        menuBtn.classList.remove("active");
        overlay.classList.remove("active");
    });
});

//Accordion Panel
var acc = document.getElementsByClassName("accordion");
var i;

for (i = 0; i < acc.length; i++) {
  acc[i].addEventListener("click", function() {
    /* Alternar entre adicionar e remover a classe "active", para destacar o botão que controla o painel */
    this.classList.toggle("active-accordion");
	
    /* Alternar entre ocultar e mostrar o painel ativo */
    var panel = this.nextElementSibling;
    if (panel.style.display === "block") {
      panel.style.display = "none";
    } else {
      panel.style.display = "block";
    }
  });
}

// Modal
function OpenModal(modalId){
  const modal = document.getElementById(modalId);
  if (modal){
    modal.style.display = "flex";
    setTimeout(() => {
      modal.classList.add('active');
    }, 10); // Delay pequeno para evitar conflito com o clique
  }
}

// Função para fechar a modal
function closeModal(modalId) {
  const modal = document.getElementById(modalId);
  const inputs = modal.querySelectorAll("input, textarea, select");
  // const contents = modal.querySelectorAll("*");
  
  if (modal){
    modal.classList.remove('active');
    modal.style.display = "none";
    if (modalId != "modal-treinamentoE") {
      inputs.forEach(input => {
        input.value = "";
      });
    }; 
  }
}


// Detect click outside modal content to close

document.addEventListener("click", function (event) {
  
  document.querySelectorAll(".modal.active").forEach(modal => {
      const modalContent = modal.querySelector(".modal-content");
      if (modalContent && !modalContent.contains(event.target)) {
          closeModal(modal.id); // pass the ID of the modal to close
      }
  });
});



document.addEventListener("keydown", function (event) {
  if (event.key === "Escape") {
      document.querySelectorAll(".modal.active").forEach(modal => {
          closeModal(modal.id); // close all active modals
      });
  }
});



//Adicionar e Excluir Conteúdos
function addTreinamento() {
  
  if (!file) {
      alert("Selecione um arquivo primeiro!");
      return;
  }

  const formData = new FormData();
  formData.append("file", file);

  fetch("/api/upload/treinamentos", {
    method: "POST",
    body: formData
  })
  .then(response => response.json())
  .then(data => alert(data.message))
  .catch(error => console.error("Erro:", error));

  // Fechar o modal após adicionar o treinamento
  closeModal();

}

const dropContainer = document.getElementById("dropcontainer");
const imagesInput = document.getElementById("images-treinamento");

if (dropContainer && imagesInput) {
    dropContainer.addEventListener("click", function () {
        imagesInput.click();
    });
}


