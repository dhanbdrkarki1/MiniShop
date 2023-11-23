function formatDateTime(dateString) {
    const parsedDate = new Date(dateString);
    const year = parsedDate.getFullYear().toString().slice(-2);
    const month = parsedDate.toLocaleString('default', { month: 'short' });
    const day = parsedDate.getDate().toString().padStart(2, '0');

    let hours = parsedDate.getHours();
    const minutes = parsedDate.getMinutes().toString().padStart(2, '0');
    const seconds = parsedDate.getSeconds().toString().padStart(2, '0');

    let amPM = hours >= 12 ? 'PM' : 'AM';
    if (hours > 12) {
        hours -= 12;
    }

    hours = hours.toString().padStart(2, '0');

    return `${day}-${month}-${year} ${hours}:${minutes}:${seconds} ${amPM}`;
}