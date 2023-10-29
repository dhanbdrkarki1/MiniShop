var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblProductData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'name', "width": "20%" },
            { data: 'category.name', "width": "10%" },
            { data: 'description', "width": "30%" },
            { data: 'price', "width": "10%" },
            { data: 'stockQuantity', "width": "10%" },
            { data: 'createdDate', "width": "15%" },
            { data: 'modifiedDate', "width": "15%" },
            { 
                data: 'productId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit </a>
                        <a onClick=Delete('/admin/product/delete?id=${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete </a>
                        </div>
                    `
                },
                "width": "15%"
            }
        ]
    });
}


function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toaster.success(data.message)
                }
            })
        }
    })
}
