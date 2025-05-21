export const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    if (isNaN(date.getTime())) return "Data inválida";

    const pad = (n, size = 2) => n.toString().padStart(size, "0");

    const day = pad(date.getDate());
    const month = pad(date.getMonth() + 1);
    const year = date.getFullYear();

    const hours = pad(date.getHours());
    const minutes = pad(date.getMinutes());
    const seconds = pad(date.getSeconds());
    const millis = pad(date.getMilliseconds(), 4);

    return `${day}/${month}/${year} ${hours}:${minutes}:${seconds}:${millis}`;
};
