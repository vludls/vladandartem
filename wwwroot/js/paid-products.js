//выкатывание продуктов
/*$(document).ready(function () {
    $('.order > div:not(:first-child)').hide();
    $('.open-order').each(function () {
        $(this).on('click', function () {
            $(this).children().toggleClass('fa-chevron-circle-down');
            $(this).children().toggleClass('fa-chevron-circle-up');
            $(this).parent().next().slideToggle();
        })
    })
})*/
//выкатывание продуктов

new Vue ({
    el: '#paid-products',
    data: {
        orders: []
    },
    mounted: function () {
        axios
            .post('/PersonalArea/GetPaidProducts', null, {
                params: {
                    start: 0
                } 
            })
            .then(response => {
                this.orders = response.data;
                
            });
    }
})