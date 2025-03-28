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
  document.getElementById("descricao").value = "";
  document.getElementById("datainicio").value = "";
  document.getElementById("datafim").value = "";
  document.getElementById("images").value = "";
}

// Fecha a modal ao clicar no botão "SAIR"
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
  // Capturar os valores dos inputs
  const nome = document.getElementById("nome").value;
  const descricao = document.getElementById("descricao").value;
  const dataInicio = document.getElementById("datainicio").value;
  const dataFim = document.getElementById("datafim").value;
  const imagemInput = document.getElementById("images");
  
  // Verificar se um arquivo de imagem foi selecionado
  let imagemSrc = "~/images/image 1.png"; // Imagem padrão caso nenhuma seja escolhida
  if (imagemInput.files.length > 0) {
      const imagemURL = URL.createObjectURL(imagemInput.files[0]); 
      imagemSrc = imagemURL;
  }

  // Criar um novo item de lista (li)
  const newListItem = document.createElement("li");
  newListItem.classList.add("list-andamento");

  // Montar o HTML dinâmico
  newListItem.innerHTML = `
      <a href="#">
          <div class="moldura">
              <img src="${imagemSrc}" alt="Imagem do Treinamento">
          </div>
          <div class="card-content">
              <div class="card-title">${nome}</div>
              <div class="card-date">DATA INÍCIO: ${dataInicio}</div>
              
          </div>
      </a>
  `;

  // Adicionar o novo item à lista de treinamentos
  document.querySelector(".bloco_treinamento").appendChild(newListItem);

  // Fechar o modal após adicionar o treinamento
  closeModal();

  // Limpar os campos do formulário
  document.getElementById("nome").value = "";
  document.getElementById("descricao").value = "";
  document.getElementById("datainicio").value = "";
  document.getElementById("datafim").value = "";
  document.getElementById("images").value = "";
}
