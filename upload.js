const fileInput = document.getElementById('file-input');
const fileList = document.getElementById('file-list');

const fileIcons = {
    "image": "📷",
    "pdf": "📄",
    "word": "📝",
    "excel": "📊",
    "powerpoint": "📽️",
    "zip": "📦",
    "video": "🎥",
    "audio": "🎵",
    "text": "📜",
    "default": "📁"
};

function getFileIcon(file) {
    const ext = file.name.split('.').pop().toLowerCase();
    if (["jpg", "jpeg", "png", "gif", "bmp"].includes(ext)) return fileIcons.image;
    if (["pdf"].includes(ext)) return fileIcons.pdf;
    if (["doc", "docx"].includes(ext)) return fileIcons.word;
    if (["xls", "xlsx"].includes(ext)) return fileIcons.excel;
    if (["ppt", "pptx"].includes(ext)) return fileIcons.powerpoint;
    if (["zip", "rar", "7z"].includes(ext)) return fileIcons.zip;
    if (["mp4", "avi", "mov", "mkv"].includes(ext)) return fileIcons.video;
    if (["mp3", "wav", "ogg"].includes(ext)) return fileIcons.audio;
    if (["txt", "md"].includes(ext)) return fileIcons.text;
    return fileIcons.default;
}

fileInput.addEventListener('change', function() {
    fileList.innerHTML = '';
    for (const file of fileInput.files) {
        const listItem = document.createElement('li');
        listItem.classList.add('file-item');

        const icon = document.createElement('span');
        icon.textContent = getFileIcon(file);
        
        const text = document.createElement('span');
        text.textContent = ` ${file.name}`;

        const link = document.createElement('a');
        link.href = URL.createObjectURL(file);
        link.target = "_blank";
        link.appendChild(icon);
        link.appendChild(text);

        listItem.appendChild(link);
        fileList.appendChild(listItem);
    }

    fileList.style.maxHeight = fileList.scrollHeight + "px";
});
