//alert(window.location.search.substr(11))
new Vue ({
    el: '#product',
    data: {
        product: []
    },
    mounted: function () {
        axios
          .post('/Home/ProductGetInfo', null, {
              params: {
                productId: window.location.search.substr(11)
              }
          })
          .then(response => {
            this.product = response.data
          });
      },
})