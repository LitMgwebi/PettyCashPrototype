import store from '@/store/store'
import axios from 'axios'
import { ref } from 'vue'

export function getDepartments() {
    store.commit('setLoading')
    const departments = ref([])
    axios({
        method: 'GET',
        url: 'Departments/index'
    })
        .then((res) => (departments.value = res.data))
        .catch((error) => store.dispatch('setStatus', error.response.data))
        .finally(() => store.commit('doneLoading'))

    return { departments }
}
