<template>
	<h3>Requisitions requiring recommendation</h3>
	<div v-for="requisition in requisitions" :key="requisition.requisitionId">
		<span>
			{{ requisition.applicant.fullName }} - R{{ requisition.amountRequested }} ({{
				requisition.glaccount.name
			}}) - {{ requisition.description }}</span
		>
		<div>
			<router-link
				:to="{
					name: 'requisition_details',
					params: {
						id: requisition.requisitionId
					}
				}"
			>
				<button>Details</button>
			</router-link>
		</div>
	</div>
</template>

<script setup>
import { getRequisitions, editRequisition } from '@/hooks/requisitionCRUD'
import { ref, inject } from 'vue'

const getRequisitionStates = inject('getRequisitionStates')
const { requisitions } = getRequisitions(getRequisitionStates.Recommendation)
</script>
