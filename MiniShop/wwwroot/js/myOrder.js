var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblMyOrderData').DataTable({
        "ajax": { url: '/customer/myorder/getall' },
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
        ]
    });
}

