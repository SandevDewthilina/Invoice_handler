import {createApp} from '../lib/vue/dist/vue.esm-browser.js';
import {DocStatus, LastChange, Document, MenuItem, SideMenuCategory, sideMenuMixin} from "./manage.js";

const app = createApp({
    data() {
        return {
            form: {
                template_name: '',
                templateRegexList: [
                    {
                        id : 1,
                        key: 'Supplier',
                        value: ''
                    }
                ]
            }
        }
    },
    computed: {
        getForm() {
            return this.form
        },
        getName() {
            return this.form.template_name
        },
        getRegexComponents() {
            console.log(this.form.templateRegexList)
            return this.form.templateRegexList
        }
    },
    methods: {
        addRow() {
            let regexList = this.form.templateRegexList
            let lastId = -1;
            if(regexList.length > 0) {
                lastId = regexList[regexList.length - 1].id
            }
            const newId = lastId + 1;
            regexList.push({
                id: newId,
                key: '',
                value: ''
            })
            this.form.templateRegexList = regexList
        },
        deleteRow(id) {
            console.log(id)
            this.form.templateRegexList.splice(id, 1)
        },
        submit(e) {
            e.preventDefault()
            let supplierElement = this.form.templateRegexList.find(i => i.id === 1)
            if (supplierElement.value === '') {
                alert("please enter regex for the Supplier Tag")
                return
            }
            axios.post('/api/MasterDataApi/CreateTemplate', this.form).then(resp => {
                if(resp.data.success) {
                    window.location.href = '/Home/ViewTemplates'
                } else {
                    alert('request was not succesful')
                }
            }).catch(err => {
                console.log(err.message)
                alert(err.message)
            })
        }
    },
    created() {
        
    }
});

app.mount("#main");