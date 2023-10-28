
$(document).ready(function () {
    LoadDataTable();
});

function LoadDataTable(){
   dataTable = $('#TableData').DataTable({
        "ajax": { url : '/admin/products/getall'},
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'price', "width": "10%" },
            { data: 'author', "width": "20%" },
            { data: 'catagory.name', "width": "15%" }
        ]
    });
}
/*
    The Columns that we have added here should have the same name as the columns returned by the
    API in the json format through ajax. It's Case sensitive.
*/