<template>
	<v-container>
		<v-row>
			<v-col>
				<h2>Transactions</h2>
			</v-col>
		</v-row>
		<v-row>
			<v-col>
				<v-data-table-server
					v-model:items-per-page="options.itemsPerPage"
					v-model:page="options.page"
					:headers="headers"
					:items="paginatedItems"
					:items-length="totalItems"
				>
					<template v-slot:top>
						<v-dialog v-model="dialog" max-width="500px">
							<RequisitionDialog
								:requisitionId="selectedId"
								@closeDialog="closeDialog"
							/>
						</v-dialog>
						<v-col>
							<label>Filter</label>
							<select
								:disabled="arrayOfTypes.length == 0"
								v-model="transactionFilter"
							>
								<option v-for="type in arrayOfTypes" :value="type" :key="type">
									{{ type.type }}
								</option>
							</select>
						</v-col>
					</template>
					<template v-slot:item="{ item }">
						<tr>
							<td>{{ item.transactionId }}</td>
							<td>{{ formatAmount(item.amount) }}</td>
							<td>{{ item.transactionType }}</td>
							<td>{{ formatDate(item.transactionDate) }}</td>
							<td v-if="item.requisition != null">
								<button @click="viewRequisition(item)">
									{{ item.requisition.applicant.fullName }}
								</button>
							</td>
							<td v-else-if="item.depositor != null">
								{{ item.depositor.fullName }}
							</td>
							<td v-else>Uknown</td>
						</tr>
					</template>
				</v-data-table-server>
			</v-col>
			<v-col>
				<aside v-if="vault">
					<h3>Vault</h3>
					<div>Current Amount: {{ formatAmount(vault.currentAmount) }}</div>
				</aside>
				<aside>
					<section class="create">
						<h3>Deposit Money</h3>
						<form @submit.prevent="addSubmit">
							<div>
								<label>Amount: </label>
								<input type="text" v-model="newTransaction.amount" />
							</div>
							<div>
								<v-btn type="submit">Deposit</v-btn>
								<v-btn @click="() => (newTransaction.amount = 0)"> Cancel </v-btn>
							</div>
						</form>
					</section>
				</aside>
			</v-col>
		</v-row>
	</v-container>
</template>

<script setup>
import { getTransactions, addTransaction } from '@/hooks/transactionCRUD'
import RequisitionDialog from '@/components/Transaction/RequisitionDialog.vue'
import { getVault } from '@/hooks/vaultCRUD'
import { ref, inject, watch } from 'vue'
import moment from 'moment'

//#region GET call

const typeOfTransaction = inject('typeOfTransaction')

const { transactions, getter: transactionGetter } = getTransactions()
const { vault, getter: vaultGetter } = getVault()

const transactionFilter = ref({
	type: 'All'
})
const arrayOfTypes = ref([
	{ type: typeOfTransaction.All },
	{ type: typeOfTransaction.Deposit },
	{ type: typeOfTransaction.Change },
	{ type: typeOfTransaction.Withdrawal }
])

watch(
	transactions,
	async (oldTransactions, newTransactions) => {
		await transactionGetter(transactionFilter.value.type)
		await vaultGetter()
		updateTableData()
	},
	{ immediate: true, deep: true }
)

//#endregion

//#region pagination and ordering datatable

const paginatedItems = ref([]) // Data to show in the table
const totalItems = ref(0)

const headers = [
	{ title: 'ID', key: 'transactionId' },
	{ title: 'Amount', key: 'amount' },
	{ title: 'Type', key: 'transactionType' },
	{ title: 'Date', key: 'transactionDate' },
	{ title: 'User', key: '' }
]
const options = ref({
	page: 1,
	itemsPerPage: 5,
	sortBy: [],
	sortDesc: []
})

const updateTableData = () => {
	let sortedItems = [...transactions.value]
	totalItems.value = transactions.value.length
	// Handle sorting
	if (options.value.sortBy.length > 0) {
		const sortKey = options.value.sortBy[0]
		const sortDesc = options.value.sortDesc[0]

		sortedItems.sort((a, b) => {
			if (a[sortKey] < b[sortKey]) return sortDesc ? 1 : -1
			if (a[sortKey] > b[sortKey]) return sortDesc ? -1 : 1
			return 0
		})
	}

	// Handle pagination
	const start = (options.value.page - 1) * options.value.itemsPerPage
	const end = start + options.value.itemsPerPage
	paginatedItems.value = sortedItems.slice(start, end)
}

//#endregion

//#region Make new Transaction

const addSubmit = () => {
	addTransaction(newTransaction.value)
	newTransaction.value.amount = 0
}

const newTransaction = ref({
	amount: 0,
	transactionType: typeOfTransaction.Deposit,
	vaultId: vault.vaultId
})

//#endregion

//#region Handling requisition dialog

const dialog = ref(false)
const selectedId = ref()
const viewRequisition = (item) => {
	selectedId.value = item.requisitionId
	dialog.value = true
}
const closeDialog = () => (dialog.value = false)

//#endregion

//#region Formatting data output

function formatDate(date) {
	if (date) return moment(String(date)).format('DD-MM-YYYY')
}
function formatAmount(num) {
	return new Intl.NumberFormat('en-ZA', {
		style: 'currency',
		currency: 'ZAR'
	}).format(num)
}

//#endregion
</script>
