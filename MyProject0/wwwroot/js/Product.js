var dataTable
$(document).ready(function () {
    LoadDataTable();
});
/*
    The Columns that we have added here should have the same name as the columns returned by the
    API in the json format through ajax. It's Case sensitive.
*/
function LoadDataTable(){
   dataTable = $('#TableData').DataTable({
        "ajax": { url : '/admin/products/getall'},
        "columns": [
            { data: 'title', "width": "20%" },
            { data: 'isbn', "width": "15%" },
            { data: 'price', "width": "10%" },
            { data: 'author', "width": "20%" },
            { data: 'catagory.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                    <div class = "w-75 btn-group" role="group"> 
                        <a href="/admin/products/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i> Edit </a>
                        <a onClick = Delete('/admin/products/delete?id=${data}') class="btn btn-danger mx-2"> <i class="bi bi-trash3-fill"></i> Delete </a>
                    </div>`
                },
                "width":"25%"
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
            Swal.fire(
                'Deleted!',
                'Your file has been deleted.',
                'success',
                $.ajax({
                    url: url,
                    type: 'DELETE',
                    success: function (data) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                })
            )
        }
    })
}