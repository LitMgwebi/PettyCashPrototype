import store from '@/store/store.js'
import axios from 'axios'
import { ref } from 'vue'

export function getApprovalStatuses() {
    store.commit('setLoading')
    const statuses = ref([])
    axios({
        method: 'GET',
        url: 'Statuses/get_approved'
    })
        .then((res) => (statuses.value = res.data))
        .catch((error) => store.dispatch('setStatus', error.response.data))
        .finally(() => store.commit('doneLoading'))

    return { statuses }
}

export function getRecommendationStatuses() {
    store.commit('setLoading')
    const statuses = ref([])
    axios({
        method: 'GET',
        url: 'Statuses/get_recommended'
    })
        .then((res) => (statuses.value = res.data))
        .catch((error) => store.dispatch('setStatus', error.response.data))
        .finally(() => store.commit('doneLoading'))

    return { statuses }
}

export function getStatesStatuses() {
    store.commit('setLoading')
    const statuses = ref([])
    axios({
        method: 'GET',
        url: 'Statuses/get_requisition_states'
    })
        .then((res) => (statuses.value = res.data))
        .catch((error) => store.dispatch('setStatus', error.response.data))
        .finally(() => store.commit('doneLoading'))

    return { statuses }
}

export function getAllStatuses() {
    store.commit('setLoading')
    const statuses = ref([])
    axios({
        method: 'GET',
        url: 'Statuses/index'
    })
        .then((res) => (statuses.value = res.data))
        .catch((error) => store.dispatch('setStatus', error.response.data))
        .finally(() => store.commit('doneLoading'))

    return { statuses }
}
