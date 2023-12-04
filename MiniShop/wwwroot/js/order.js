var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblOrderData').DataTable({
        "ajax": { url: '/admin/order/getall' },
        "columns": [
            { data: 'orderId', "width": "10%" },
            { data: 'name', "width": "20%" },
            { data: 'phoneNumber', "width": "10%" },
            { data: 'applicationUser.email', "width": "20%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'paymentStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'orderDate',
                "render": function (data) {
                    return formatDateTime(data);
                },
                "width": "20%"
            },
            { 
                data: 'orderId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/order/details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>
                        </div>
                    `
                },
                "width": "15%"
            }
        ],
        responsive: true
    });
}

