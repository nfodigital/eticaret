var handleDataTableButtons = function () {
    "use strict";
    0 !== $(".dataTable").length && $(".dataTable").DataTable({
        "ordering": false,
        "colVis": {
            "buttonText": "Change columns"
        },
        dom: "Bfrtip",
        buttons: [{ extend: "copy", className: "btn-sm" },
            { extend: "csv", className: "btn-sm" },
            { extend: "excel", className: "btn-sm" },
            { extend: "pdf", className: "btn-sm" },
            { extend: "print", className: "btn-sm" }],
        responsive: !0,
        
    })
}, TableManageButtons = function () { "use strict"; return { init: function () { handleDataTableButtons() } } }();