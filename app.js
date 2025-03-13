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