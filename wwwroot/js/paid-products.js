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
Vue.directive('scroll', {
    inserted: function (el, binding) {
      let f = function (evt) {
        if (binding.value(evt, el)) {
          window.removeEventListener('scroll', f)
        }
      }
      window.addEventListener('scroll', f)
    }
  });

new Vue ({
    el: '#paid-products',
    data: {
        orders: [],
        scroll: 0,
        from: 0
    },
    mounted: function () {
        axios
            .post('/PersonalArea/GetPaidProducts', null, {
                params: {
                    start: this.from
                } 
            })
            .then(response => {
                this.orders = response.data;
                
            });
        this.scroll += 5000;
        this.from += 20
    },
    methods: {
        handleScroll: function () {
            axios
                .post('/PersonalArea/GetPaidProducts', null, {
                    params: {
                        start: this.from
                    } 
                })
                .then(response => {
                    //this.orders = response.data;
                    for(var key in response.data) {
                        this.orders.push(response.data[key]);
                    }
                    
                });
            this.scroll += 5000;
            this.from += 20
        },
        scrolling: function () {
            if (window.scrollY > this.scroll) {
                this.handleScroll()
            }
        }
    }
})