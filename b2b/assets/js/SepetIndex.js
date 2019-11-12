// JavaScript source code
renewRows();
$(document).ready(function () {
    var isBrowserCompatible =
        $('html').hasClass('ua-ie-10') ||
        $('html').hasClass('ua-webkit') ||
        $('html').hasClass('ua-firefox') ||
        $('html').hasClass('ua-opera') ||
        $('html').hasClass('ua-chrome');

    if (isBrowserCompatible) {
        window.card = new Skeuocard($("#skeuocard"), {
            debug: false
        });
    }
});
$('#Semt').on('change', function () {
    console.log($(this).val());
});
function renewRows() {
    $.get('/Sepet/SepetHesabi', {
    }, function (d) {
        $('#kdv').html("+ " + d.KdvTutari.toFixed(2) + " ₺");
        $('#toplamTutar').html(d.ToplamTutar.toFixed(2) + " ₺");
        $('#aratoplam').html(d.AraToplam.toFixed(2) + " ₺");
        $('#genelTutar').html(d.GenelTutar.toFixed(2) + " ₺");
        genelTutar = d.GenelTutar.toFixed(2);
        //   $('#iskontoTutar').html("-" + d.IskontoTutari.toFixed(2) + " ₺");
    });
}
function odemeYap() {
    $.ajax({
        url: "/Sepet/odemeYap",
        data: {
            'Sehir': $('#Semt').val(), 'AcikAdres': $('#acikadres').val(), 'AdresTarifi': $('#adrestarif').val(),
            'Notlar': $('#Notlar').val(), 'AliciAdSoyad': $('#aliciadsoyad').val(),
            'SiparisVerenAdSoyad': $('#SiparisVeren').val(),
            'SiparisVerenTelefon': $('#iletisimNo').val(),
            'PosBilgileri.kartSahibi': $('#cc_name').val(),
            'PosBilgileri.kartNumarasi': $('#cc_number').val(),
            'PosBilgileri.guvenlikKodu': $('#cc_cvc').val(),
            'PosBilgileri.ay': $('#cc_exp_month').val(),
            'PosBilgileri.yil': $('#cc_exp_year').val(), 'PosBilgileri.tutar': genelTutar, 'PosBilgileri.taksit': '',
        },
        success: function (d) {
            if (d.Error == "success") {
                $('#threedpage').modal('show');
                $('#ReturnMsg').html(d.ReturnMsg);
            }
            else {
              //  window.location.href = "/Siparis/Problem"
            }
        }
    });
}