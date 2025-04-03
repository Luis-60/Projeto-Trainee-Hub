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

//Abrir Modal
const modal = document.querySelector('.modal');
const btnAbrir = document.querySelector('.btnAbrirAdd'); // Botão para abrir a modal
const btnFechar = document.querySelector('.btnClose'); // Botão para fechar a modal
const modalContent = document.querySelector('.modal-treinamento'); // Conteúdo da modal

// Função para abrir a modal
function AbrirAdd() {
  modal.classList.add('active');
}

// Função para fechar a modal
function closeModal() {
  modal.classList.remove('active');
  document.getElementById("nome").value = "";
  document.getElementById("instrutor").value = "";
  document.getElementById("descricao").value = "";
  document.getElementById("datainicio").value = "";
  document.getElementById("datafim").value = "";
  document.getElementById("images-treinamento").value = "";
}

btnFechar.addEventListener('click', closeModal);

// Fecha a modal ao clicar fora do conteúdo dela
modal.addEventListener('click', function (event) {
    if (!modalContent.contains(event.target)) {
        closeModal();
    }
});

// Adiciona evento ao botão de abrir a modal
btnAbrir.addEventListener('click', AbrirAdd);

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

  // Limpar os campos do formulário
  document.getElementById("nome").value = "";
  document.getElementById("descricao").value = "";
  document.getElementById("instrutor").value = "";
  document.getElementById("datainicio").value = "";
  document.getElementById("datafim").value = "";
  document.getElementById("images-treinamento").value = "";
}

document.getElementById("dropcontainer").addEventListener("click", function() {
  document.getElementById("images-treinamento").click(); // Simula o clique no input
});