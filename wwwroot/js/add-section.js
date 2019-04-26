new Vue ({
    el: '#sections',
    data: {
        sections: [],
        modalId: 'id',
        sectionId: '',
        sectionName: '',
        index: '',
        addSectionName: ''
    },
    mounted: function () {
        axios
            .post('/AdminMenu/Section/Api/GetAll')
            .then(response => {
                this.sections = response.data;
            });
    },
    methods: {
        activateModal: function (sectionId, sectionName, index) {
            this.modalId = 'id' + sectionId;
            this.sectionName = sectionName;
            this.sectionId = sectionId;
            this.index = index
        },
        deleteSection: function (event) {
            axios
            .post('/AdminMenu/Section/Api/Delete', null, { 
                params: { 
                    sectionId: this.sectionId
                } 
            })
            .then(
                this.sections.splice(this.index, 1)
            );
            event.preventDefault();
        },
        closeModal: function () {
            $('.close-modal').trigger('click');
        },
        addSection: function (event) {
            axios
            .post('/AdminMenu/Section/Api/Add', null, { 
                params: { 
                    sectionName: this.addSectionName
                }
            })
            .then(response => {
                this.sections.push(response.data);
                this.addSectionName = ''
            });
            event.preventDefault();
        }
    }
})
