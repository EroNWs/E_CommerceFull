var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/adminpanel/product/getall' },
        "columns": [
            { data: 'title', "width": "20%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "20%" },
            { data: 'category.name', "width": "20%" },       
     
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/adminpanel/product/upsert?id=${data}" class="btn btn-delete mx-2"><i class="bi bi-pencil"></i> Edit</a>               
                     <a onClick=Delete('/adminpanel/product/delete/${data}') class="btn btn-delete mx-2"><i class="bi bi-trash2"></i> Delete</a>
                    </div>`
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
                    toastr.success(data.message);
                }
            })
        }
    })
}